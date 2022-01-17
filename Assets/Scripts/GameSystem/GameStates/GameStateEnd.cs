using DAE.ReplaySystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem
{
    class GameStateEnd : GameStateBase
    {
        private Screen _endScreen;
        private Screen _playScreen;

        public GameStateEnd(StateMachine<GameStateBase> stateMachine, Screen endScreen, Screen playScreen) : base(stateMachine)
        {
            _endScreen = endScreen;
            _playScreen = playScreen;
        }

        public override void OnEnter()
        {
            _playScreen.Disable();
            _endScreen.Enable();
        }

        public override void OnExit()
        {
            _endScreen.Disable();
        }
    }
}
