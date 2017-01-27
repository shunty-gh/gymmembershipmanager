using System;
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
    }
}
