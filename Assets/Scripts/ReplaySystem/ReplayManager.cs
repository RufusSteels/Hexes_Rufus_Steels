using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.ReplaySystem
{
    public class ReplayManager
    {
        private List<IReplayCommand> _replayCommands = new List<IReplayCommand>();
        private int _currentCommand = -1;

        public bool IsAtEnd => _currentCommand >= _replayCommands.Count - 1;
        public void Execute(IReplayCommand replayCommand)
        {
            _replayCommands.Add(replayCommand);

            Forward();
        }

        public void BackWard()
        {
            if(_currentCommand < 0)
                return;

            _replayCommands[_currentCommand].Backward();
            _currentCommand -= 1;
        }

        public void Forward()
        {
            if (IsAtEnd)
                return;

            _currentCommand += 1;
            _replayCommands[_currentCommand].Forward();
        }
    }
}
