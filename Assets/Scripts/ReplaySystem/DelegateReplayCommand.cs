using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.ReplaySystem
{
    public class DelegateReplayCommand : IReplayCommand
    {
        private Action _forward;
        private Action _backward;

        public DelegateReplayCommand(Action forward, Action backward)
        {
            _forward = forward;
            _backward = backward;
        }

        public void Backward()
            => _backward();

        public void Forward()
            => _forward();
    }
}
