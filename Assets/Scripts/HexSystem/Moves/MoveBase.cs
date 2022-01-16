using DAE.BoardSystem;
using DAE.ReplaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    abstract class MoveBase<TPosition> : IMove<TPosition>
        where TPosition : IPosition
    {
        protected Board<Character<TPosition>, TPosition> Board { get; set; }
        protected Grid<TPosition> Grid { get; set; }
        protected ReplayManager ReplayManager { get; set; }

        protected MoveBase(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager)
        {
            Grid = grid;
            Board = board;
            ReplayManager = replayManager;
        }
        public bool CanExecute(Character<TPosition> character)
            => true;

        public abstract void Execute(Character<TPosition> character, List<TPosition> positions);

        public abstract List<TPosition> ValidPositions(Character<TPosition> character);

        public abstract List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position);
    }
}
