using DAE.BoardSystem;
using DAE.Commons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAE.HexSystem
{
    public class MoveManager <TPosition> 
        where TPosition : IPosition
    {
        private MultiValueDictionary<PieceType, IMove<TPosition>> _moves = new MultiValueDictionary<PieceType, IMove<TPosition>>();

        private Board<Character<TPosition>, TPosition> _board;
        private Grid<TPosition> _grid;

        public MoveManager(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid)
        {
            _board = board;

            _board.PiecePlaced += (s, e) => e.Piece.PlaceAt(e.AtPosition);
            _board.PieceTaken += (s, e) => e.Piece.TakeFrom(e.Position);
            _board.PieceMoved += (s, e) => e.Piece.MoveTo(e.ToPosition);

            _grid = grid;

            //_moves.Add(
            //    PieceType.Pawn, 
            //        new ConfigurableMove<TPosition>(board, grid, 
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .North(1, MovementHelper<TPosition>.Empty)
            //                            .NorthEast(1, MovementHelper<TPosition>.ContainsEnemy)
            //                            .NorthWest(1, MovementHelper<TPosition>.ContainsEnemy)
            //                            .CollectValidPositions()));
            //
            //_moves.Add(
            //    PieceType.Queen,
            //        new ConfigurableMove<TPosition>(board, grid,
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .North()
            //                            .NorthEast()
            //                            .East()
            //                            .SouthEast()
            //                            .South()
            //                            .SouthWest()
            //                            .West()
            //                            .NorthWest()
            //                            .CollectValidPositions()));
            //
            //_moves.Add(
            //    PieceType.King,
            //        new ConfigurableMove<TPosition>(board, grid,
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .North(1)
            //                            .NorthEast(1)
            //                            .East(1)
            //                            .SouthEast(1)
            //                            .South(1)
            //                            .SouthWest(1)
            //                            .West(1)
            //                            .NorthWest(1)
            //                            .CollectValidPositions()));
            //
            //_moves.Add(
            //    PieceType.Bishop,
            //        new ConfigurableMove<TPosition>(board, grid,
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .NorthEast()
            //                            .SouthEast()
            //                            .SouthWest()
            //                            .NorthWest()
            //                            .CollectValidPositions()));
            //
            //_moves.Add(
            //    PieceType.Rook,
            //        new ConfigurableMove<TPosition>(board, grid,
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .North()
            //                            .East()
            //                            .South()
            //                            .West()
            //                            .CollectValidPositions()));
            //
            //_moves.Add(
            //    PieceType.Knight,
            //        new ConfigurableMove<TPosition>(board, grid,
            //            (b, g, p) => new MovementHelper<TPosition>(b, g, p)
            //                            .KnightMoves(1)
            //                            .CollectValidPositions()));
            ////_moves.Add(PieceType.Pawn, new PawnDoubleMove(board, grid));
        }

        public List<TPosition> ValidPositionsFor(Character<TPosition> piece)
        {
            List<TPosition> result = _moves[piece.PieceType]
                .Where((m) => m.CanExecute(piece))
                .SelectMany((m) => m.Positions(piece))
                .ToList();
            //get all executable Moves
            //foreach move 
            //  get/collect positions
            //return positions

            return result;
        }

        public void Move(Character<TPosition> piece, TPosition position)
        {
            var move = _moves[piece.PieceType]
                .Where(m => m.CanExecute(piece))
                .Where(m => m.Positions(piece).Contains(position))
                .First();

            move.Execute(piece, position);
            //get first executable moves
            //where validMoves contains position
            //execute move
        }
    }
}
