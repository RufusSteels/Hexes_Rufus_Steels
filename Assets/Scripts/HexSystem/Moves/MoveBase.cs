using DAE.BoardSystem;
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

        protected MoveBase(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid)
        {
            Grid = grid;
            Board = board;
        }
        public bool CanExecute(Character<TPosition> character)
            => true;

        public abstract void Execute(Character<TPosition> character, List<TPosition> positions);
        //{
        //    foreach(TPosition position in positions)
        //    {
        //        //Staat er een Character op Position?
        //        if (Board.TryGetPiece(position, out Character<TPosition> toPiece))
        //            //Capture Character
        //            Board.Take(toPiece);
        //
        //        //Move to position
        //        if (_moves)
        //            Board.Move(character, position);
        //    }
        //}

        public abstract List<TPosition> ValidPositions(Character<TPosition> character);

        public abstract List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position);
    }
}
