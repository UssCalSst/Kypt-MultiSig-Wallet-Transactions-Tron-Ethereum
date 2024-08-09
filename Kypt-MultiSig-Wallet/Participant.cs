using NBitcoin;
using System;
using System.Collections.Generic;

namespace MixerFront
{
    public class Participant
    {
        public string SessionId;
        private readonly object _lock = new object();

        private bool _isReady = false;

        public string WithdrawalAddress = "";
        public string DepositAddress = "";

        private string _signedTransaction = "";

        public Script RedeemScript;

        public List<Coin> Coins;
        public DateTime Created;

        private Dictionary<string, decimal> _outputMapping =
            new Dictionary<string, decimal>();

        public Participant(string sessionId)
        {
            SessionId = sessionId;
            Created = DateTime.UtcNow;
        }

        public bool Ready(bool isReady)
        {
            lock (_lock)
            {
                if (WithdrawalAddress == "")
                    return false;
                if (DepositAddress == "")
                    return false;
                if (_outputMapping.Count <= 0)
                    return false;

                _isReady = isReady;
                return _isReady;
            }
        }

        public Dictionary<string, decimal> GetOutputs()
        {
            // This method should be thread-safe
            lock (_lock)
            {
                return new Dictionary<string, decimal>(_outputMapping);
            }
        }

        public bool UpdateWithdrawalAddress(string address)
        {
            if (_isReady)
                return false;

            WithdrawalAddress = address;
            // TODO: Add input validation?
            return true;
        }

        public bool UpdateDepositAddress(string address)
        {
            if (_isReady)
                return false;

            DepositAddress = address;
            // TODO: Add input validation?
            return true;
        }

        public bool AddOutputAddress(string address, decimal amount)
        {
            if (_isReady)
                return false;

            // TODO: Add input validation?
            lock (_lock)
            {
                if (_outputMapping.ContainsKey(address))
                    return false;

                _outputMapping.Add(address, amount);
            }
            return true;
        }

        public bool RemoveOutputAddress(string address)
        {
            if (_isReady)
                return false;

            // TODO: Add input validation?
            lock (_lock)
            {
                if (!_outputMapping.ContainsKey(address))
                    return false;

                _outputMapping.Remove(address);
            }
            return true;
        }

        public bool UpdateOutputAddress(string address, decimal amount)
        {
            if (_isReady)
                return false;

            // TODO: Add input validation?
            lock (_lock)
            {
                if (!_outputMapping.ContainsKey(address))
                    return false;

                _outputMapping[address] = amount;
            }
            return true;
        }

        public virtual bool IsReady()
        {
            return _isReady;
        }
    }
}
