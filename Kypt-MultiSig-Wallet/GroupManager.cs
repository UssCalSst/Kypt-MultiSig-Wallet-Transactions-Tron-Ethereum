using NBitcoin;
using System;
using System.Collections.Generic;

namespace MixerFront
{
    public class GroupManager
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, Group> _groups = new Dictionary<string, Group>();

        public static NBitcoin.Network SelectedNetwork = NBitcoin.Network.Main;

        public GroupManager()
        {
        }

        public Group CreateNewGroup(string groupId, decimal amount, NBitcoin.Network network = null)
        {
            // TODO: Decide if 'amount' is acceptable based on active usage and vault balance
            if (network == null)
                network = Bitcoin.Instance.Mainnet;

            Group group = new Group(groupId, amount, network);
            if (!AddGroup(group))
                return null;

            return group;
        }

        public Group CreateNewGroup(decimal amount, NBitcoin.Network network = null)
        {
            Random rnd = new Random();
            string groupId = $"grp_{rnd.Next(0, int.MaxValue):X8}";
            return CreateNewGroup(groupId, amount, network);
        }

        public Group GetGroupBySessionId(string sessionId)
        {
            lock (_lock)
            {
                if (!_groups.ContainsKey(sessionId))
                    return null;

                return _groups[sessionId];
            }
        }

        public bool AddGroup(Group group)
        {
            lock (_lock)
            {
                if (_groups.ContainsKey(group.SessionId))
                    return false;

                _groups.Add(group.SessionId, group);
            }
            return true;
        }

        public bool RemoveGroup(Group group)
        {
            lock (_lock)
            {
                if (!_groups.ContainsKey(group.SessionId))
                    return false;

                _groups.Remove(group.SessionId);
            }
            return true;
        }
    }
}
