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
        public bool CanExecute(Character<TPosition> piece)
            => true;

        public void Execute(Character<TPosition> piece, TPosition position)
        {
            //Staat er een Piece op Position?
            if (Board.TryGetPiece(position, out Character<TPosition> toPiece))
                //Capture Piece
                Board.Take(toPiece);

            //Move to position
            Board.Move(piece, position);
        }

        public abstract List<TPosition> Positions(Character<TPosition> piece);
    }
}
