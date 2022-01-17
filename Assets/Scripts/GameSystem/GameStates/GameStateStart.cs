using DAE.ReplaySystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem
{
    class GameStateStart : GameStateBase
    {
        private Screen _startScreen;
        private Screen _playScreen;

        public GameStateStart(StateMachine<GameStateBase> stateMachine, Screen startScreen, Screen playScreen) : base(stateMachine)
        {
            _startScreen = startScreen;
            _playScreen = playScreen;
        }

        public override void OnEnter()
        {
            _startScreen.Enable();
            _playScreen.Disable();
        }

        public override void OnExit()
        {
            _startScreen.Disable();
            _playScreen.Enable();
        }

        public override void Forward()
        {
            StateMachine.MoveTo(GameStateBase.PlayingState);
        }
    }
}
