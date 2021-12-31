using DAE.BoardSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DAE.HexSystem
{
    internal class HexMovementHelper<TPosition>
        where TPosition : IPosition
    {
        private Board<Character<TPosition>, TPosition> _board;
        private Grid<TPosition> _grid;
        private Character<TPosition> _piece;
        private List<TPosition> _validPositions = new List<TPosition>();

        public HexMovementHelper(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece)
        {
            _board = board;
            _grid = grid;
            _piece = piece;
        }

        public HexMovementHelper<TPosition> TopRight(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(1, -1, maxSteps, validators); //expressionBody één lijn code voor een methode kan ook zo geschreven worden
        public HexMovementHelper<TPosition> Right(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(1, 0, maxSteps, validators);
        public HexMovementHelper<TPosition> BottomRight(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(0, 1, maxSteps, validators);
        public HexMovementHelper<TPosition> BottomLeft(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(-1, 1, maxSteps, validators);
        public HexMovementHelper<TPosition> Left(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(-1, 0, maxSteps, validators);
        public HexMovementHelper<TPosition> TopLeft(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(0, -1, maxSteps, validators);
        public HexMovementHelper<TPosition> All(params Validator[] validators)
            => CollectAll(validators);

        public HexMovementHelper<TPosition> Collect(int xOffset, int yOffset, int maxSteps = int.MaxValue, params Validator[] validators)
        {
            xOffset *= (_piece.PlayerID == 0) ? 1 : -1;
            yOffset *= (_piece.PlayerID == 0) ? 1 : -1;

            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            int nextCoordinateX = currentCoordinates.x + xOffset;
            int nextCoordinateY = currentCoordinates.y + yOffset;

           // _grid.TryGetPositionAt(
           //     currentCoordinates.x,
           //     currentCoordinates.y,
           //     out TPosition currentPosition);

            _grid.TryGetPositionAt(
                nextCoordinateX,
                nextCoordinateY,
                out TPosition nextPosition);

            int steps = 0;
            while (steps < maxSteps && nextPosition != null && validators.All((v) => v(_board, _grid, _piece, nextPosition)))
            {
                //if (_board.TryGetPiece(nextPosition, out Character<TPosition> nextPiece))
                //{
                //    if (nextPiece.PlayerID != _piece.PlayerID)
                //        _validPositions.Add(nextPosition);
                //    break;
                //}
                //else
                //{
                    _validPositions.Add(nextPosition);

                    nextCoordinateX += xOffset;
                    nextCoordinateY += yOffset;

                    _grid.TryGetPositionAt(
                        nextCoordinateX,
                        nextCoordinateY,
                        out nextPosition);
                    steps++;
                //}
            }

            return this;
        }
        public HexMovementHelper<TPosition> CollectAll(params Validator[] validators)
        {
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            var positions = _grid.GetAllPositions();
            foreach (TPosition position in positions)
            {
                if(validators.All((v) => v(_board, _grid, _piece, position)))
                {
                    _validPositions.Add(position);
                }
            }

            return this;
        }

        public delegate bool Validator(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece, TPosition toPosition);

        public List<TPosition> CollectValidPositions()
        {
            return _validPositions;
        }

        public static bool Empty(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece, TPosition toPosition)
            => !board.TryGetPiece(toPosition, out var _);

        //public static bool ContainsEnemy(Board<Piece<TPosition>, TPosition> board, Grid<TPosition> grid, Piece<TPosition> piece, TPosition toPosition)
        //    => !board.TryGetPiece(toPosition, out var toPiece) && toPiece.PlayerID != piece.PlayerID;
        public static bool ContainsEnemy(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece, TPosition toPosition)
           => board.TryGetPiece(toPosition, out var toPiece) && toPiece.PlayerID != piece.PlayerID;
    }
}