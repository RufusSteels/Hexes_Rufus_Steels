using DAE.BoardSystem;
using DAE.CardSystem;
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
        private MultiValueDictionary<CardType, IMove<TPosition>> _moves = new MultiValueDictionary<CardType, IMove<TPosition>>();

        private Board<Character<TPosition>, TPosition> _board;
        private Grid<TPosition> _grid;

        public MoveManager(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid)
        {
            _board = board;
            _board.PiecePlaced += (s, e) => e.Piece.PlaceAt(e.AtPosition);
            _board.PieceTaken += (s, e) => e.Piece.TakeFrom(e.Position);
            _board.PieceMoved += (s, e) => e.Piece.MoveTo(e.ToPosition);

            _grid = grid;

            _moves.Add(
                CardType.Teleport,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .All(HexMovementHelper<TPosition>.Empty)
                        .CollectValidPositions()));

            _moves.Add(
                CardType.Pushback,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .TopRight(1)
                        .Right(1)
                        .BottomRight(1)
                        .BottomLeft(1)
                        .Left(1)
                        .TopLeft(1)
                        .CollectValidPositions()));

            _moves.Add(
                CardType.Swipe,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .TopRight(1)
                        .Right(1)
                        .BottomRight(1)
                        .BottomLeft(1)
                        .Left(1)
                        .TopLeft(1)
                        .CollectValidPositions()));

            _moves.Add(
                CardType.Slash,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .TopRight(1)
                        .Right(1)
                        .BottomRight(1)
                        .BottomLeft(1)
                        .Left(1)
                        .TopLeft(1)
                        .CollectValidPositions()));
        }

        public List<TPosition> ValidPositionsFor(Character<TPosition> piece)
        {
            //List<TPosition> result = _moves[piece.PieceType]
            List<TPosition> result = _moves[CardType.Teleport]
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
            //var move = _moves[piece.PieceType]
            var move = _moves[CardType.Teleport]
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
