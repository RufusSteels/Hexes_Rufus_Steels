using DAE.BoardSystem;
using DAE.CardSystem;
using DAE.Commons;
using DAE.ReplaySystem;
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

        public MoveManager(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager)
        {
            _board = board;
            _board.PiecePlaced += (s, e) => e.Character.PlaceAt(e.AtPosition);
            _board.PieceTaken += (s, e) => e.Character.TakeFrom(e.Position);
            _board.PieceMoved += (s, e) => e.Character.MoveTo(e.ToPosition);

            _grid = grid;

            _moves.Add(
                CardType.Teleport,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .All(HexMovementHelper<TPosition>.Empty)
                        .CollectValidPositions(),

                        (b, g, p, pos) => new HexMovementHelper<TPosition>(b, g, p)
                        .Current(pos)
                        .CollectValidPositions(),

                        (b, g, p, pos) => 
                        { 
                            foreach (TPosition position in pos)
                            {
                                b.Move(p, position);
                            }
                        }
                        ));

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
                        .CollectValidPositions(),

                        (b, g, p, pos) => new HexMovementHelper<TPosition> (b, g, p)
                        .Rotate(pos)
                        .CollectValidPositions(),

                        (b, g, p, pos) =>
                        {
                            foreach (TPosition position in pos)
                            {
                                var pushedPosition = new HexMovementHelper<TPosition>(b, g, p)
                                .Push(position)
                                .CollectValidPositions();
                                if (b.TryGetPiece(position, out Character<TPosition> toPiece))
                                {
                                    if (pushedPosition.Count > 0)
                                        b.Move(toPiece, pushedPosition[0]);
                                    else
                                        b.Take(toPiece);
                                }
                            }
                        }
                        ));

            _moves.Add(
                CardType.Swipe,
                    new ConfigurableMove<TPosition>(board, grid,
                        (b, g, p) => new HexMovementHelper<TPosition>(b, g, p)
                        .TopRight()
                        .Right()
                        .BottomRight()
                        .BottomLeft()
                        .Left()
                        .TopLeft()
                        .CollectValidPositions(), 
                        
                        (b, g, p, pos) => new HexMovementHelper<TPosition>(b, g, p)
                        .Line(pos)
                        .CollectValidPositions(),

                        (b, g, p, pos) =>
                        {
                            foreach (TPosition position in pos)
                            {
                                if (b.TryGetPiece(position, out Character<TPosition> toPiece))
                                    b.Take(toPiece);
                            }
                        }
                        ));

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
                        .CollectValidPositions(),

                        (b, g, p, pos) => new HexMovementHelper<TPosition>(b, g, p)
                        .Rotate(pos)
                        .CollectValidPositions(),

                        (b, g, p, pos) =>
                        {
                            foreach (TPosition position in pos)
                            {
                                if (b.TryGetPiece(position, out Character<TPosition> toPiece))
                                    b.Take(toPiece);
                            }
                        }
                        ));
        }

        public List<TPosition> ValidPositionsFor(Character<TPosition> character, CardType cardType)
        {
            //List<TPosition> result = _moves[character.PieceType]
            List<TPosition> result = _moves[cardType]
                .Where((m) => m.CanExecute(character))
                .SelectMany((m) => m.ValidPositions(character))
                .ToList();
            //get all executable Moves
            //foreach move 
            //  get/collect positions
            //return positions

            return result;
        }

        public List<TPosition> AffectedPositionsFor(Character<TPosition> character, TPosition position, CardType cardType)
        {
            List<TPosition> result = _moves[cardType]
                .Where((m) => m.CanExecute(character))
                .SelectMany((m) => m.AffectedPositions(character, position))
                .ToList();

            return result;
            //List<TPosition> positions = new List<TPosition>();
            //switch (cardType)
            //{
            //    case CardType.Teleport:
            //        positions.Add(position);
            //        break;
            //    case CardType.Swipe:
            //        foreach (TPosition affectedPosition in new HexMovementHelper<TPosition>(_board, _grid, character).Line(position).CollectValidPositions())
            //            positions.Add(affectedPosition);
            //        break;
            //    case CardType.Slash:
            //        foreach (TPosition affectedPosition in new HexMovementHelper<TPosition>(_board, _grid, character).Rotate(position).CollectValidPositions())
            //            positions.Add(affectedPosition);
            //        break;
            //    case CardType.Pushback:
            //        foreach (TPosition affectedPosition in new HexMovementHelper<TPosition>(_board, _grid, character).Rotate(position).CollectValidPositions())
            //            positions.Add(affectedPosition);
            //        break;
            //}
            //return positions;
        }

        public void Execute(Character<TPosition> character, TPosition position, CardType cardType)
        {
            //var move = _moves[character.PieceType]
            var move = _moves[cardType]
                .Where(m => m.CanExecute(character))
                .Where(m => m.ValidPositions(character).Contains(position))
                .First();

            var positions = AffectedPositionsFor(character, position, cardType);
            move.Execute(character, positions);
            //get first executable moves
            //where validMoves contains position
            //execute move
        }
    }
}
