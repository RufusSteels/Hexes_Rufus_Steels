using DAE.HexSystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem
{

    abstract class GameStateBase : IState<GameStateBase>
    {
        public const string PlayingState = "Playing";
        public const string ReplayingState = "Replaying";
        public const string StartState = "Start";
        public const string EndState = "End";

        private StateMachine<GameStateBase> _stateMachine;
        public StateMachine<GameStateBase> StateMachine => _stateMachine;

        protected GameStateBase(StateMachine<GameStateBase> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }

        public virtual void Forward()
        {

        }

        public virtual void Backward()
        {

        }

        public virtual void Select(Character<Tile> character)
        {

        }

        public virtual void Select(Tile position)
        {

        }

        public virtual void DeselectAll()
        {

        }
    }
}
