using System;
using System.Collections.Generic;
using Shunty.Logging;


namespace Shunty.GymMembershipManager
{
    public abstract class MembershipAction
    {
        private IConnectionManager _manager;

        public MembershipAction(IConnectionManager manager)
        {
            _manager = manager;
        }

        protected IConnectionManager Manager { get { return _manager; } }

        public KeyValuePair<string, string> CreateQueryParam(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
