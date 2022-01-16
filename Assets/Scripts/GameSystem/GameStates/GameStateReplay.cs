using DAE.ReplaySystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem
{
    class GameStateReplay : GameStateBase
    {
        private readonly ReplayManager _replayManager;

        public GameStateReplay(StateMachine<GameStateBase> stateMachine, ReplayManager replayManager) : base(stateMachine)
        {
            _replayManager = replayManager;
        }

        public override void OnEnter()
        {
            Backward();
        }

        public override void Backward()
        {
            _replayManager.BackWard();
        }

        public override void Forward()
        {
            _replayManager.Forward();
            if (_replayManager.IsAtEnd)
                StateMachine.MoveTo(PlayingState);
        }
    }
}
