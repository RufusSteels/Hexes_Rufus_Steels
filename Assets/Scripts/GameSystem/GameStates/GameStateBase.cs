using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem.GameStates
{

    abstract class GameStateBase : IState<GameStateBase>
    {
        private StateMachine<GameStates.GameStateBase> _stateMachine;
        public StateMachine<GameStates.GameStateBase> StateMachine => _stateMachine;

        protected GameStateBase(StateMachine<GameStateBase> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            throw new NotImplementedException();
        }

        public void OnExit()
        {
            throw new NotImplementedException();
        }
    }
}
