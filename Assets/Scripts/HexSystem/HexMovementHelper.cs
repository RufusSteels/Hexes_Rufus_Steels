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

        public HexMovementHelper(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character)
        {
            _board = board;
            _grid = grid;
            _piece = character;
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
        {
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            var positions = _grid.GetAllPositions();
            foreach (TPosition position in positions)
            {
                if (validators.All((v) => v(_board, _grid, _piece, position)))
                {
                    _validPositions.Add(position);
                }
            }

            return this;
        }
        public HexMovementHelper<TPosition> Current(TPosition position, params Validator[] validators)
        {
            _validPositions.Add(position);
            return this;
        }
        public HexMovementHelper<TPosition> Rotate(TPosition position, params Validator[] validators)
        {
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            if (!_grid.TryGetCoordinateAt(position, out var selectedCoordiates))
                return this;

            (int q, int r) directionOffset = (selectedCoordiates.x - currentCoordinates.x, selectedCoordiates.y - currentCoordinates.y);
            (int q, int r) leftOffset = (-directionOffset.r, directionOffset.q + directionOffset.r);
            (int q, int r) rightOffset = (directionOffset.q + directionOffset.r, -directionOffset.q);

            _validPositions.Add(position);
            if (_grid.TryGetPositionAt(leftOffset.q + currentCoordinates.x, leftOffset.r + currentCoordinates.y, out var left))
                _validPositions.Add(left);
            if (_grid.TryGetPositionAt(rightOffset.q + currentCoordinates.x, rightOffset.r + currentCoordinates.y, out var right))
                _validPositions.Add(right);

            return this;
        }
        public HexMovementHelper<TPosition> Line(TPosition position, params Validator[] validators)
        {
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            if (!_grid.TryGetCoordinateAt(position, out var selectedCoordiates))
                return this;

            (int q, int r) direction = (selectedCoordiates.x - currentCoordinates.x, selectedCoordiates.y - currentCoordinates.y);

            if (direction.q > 0 || direction.q < 0)
                direction.q /= Math.Abs(direction.q);
            if (direction.r > 0 || direction.r < 0)
                direction.r /= Math.Abs(direction.r);

            return Collect(direction.q, direction.r);
        }
        public HexMovementHelper<TPosition> Push(TPosition position, params Validator[] validators)
        {
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            if (!_grid.TryGetCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            if (!_grid.TryGetCoordinateAt(position, out var selectedCoordiates))
                return this;

            (int q, int r) direction = (selectedCoordiates.x - currentCoordinates.x, selectedCoordiates.y - currentCoordinates.y);

            if (direction.q > 0 || direction.q < 0)
                direction.q /= Math.Abs(direction.q);
            if (direction.r > 0 || direction.r < 0)
                direction.r /= Math.Abs(direction.r);

            return Collect(2*direction.q, 2*direction.r, 1);
        }
        public HexMovementHelper<TPosition> Surround(TPosition position, params Validator[] validators)
        {
            if (!_grid.TryGetCoordinateAt(position, out var selectedCoordinates))
                return this;

            _validPositions.Add(position);
            if(_grid.TryGetPositionAt(selectedCoordinates.x + 1, selectedCoordinates.y - 1, out var newPosition0))
                _validPositions.Add(newPosition0);
            if (_grid.TryGetPositionAt(selectedCoordinates.x + 1, selectedCoordinates.y, out var newPosition1))
                _validPositions.Add(newPosition1);
            if (_grid.TryGetPositionAt(selectedCoordinates.x, selectedCoordinates.y + 1, out var newPosition2))
                _validPositions.Add(newPosition2);
            if (_grid.TryGetPositionAt(selectedCoordinates.x - 1, selectedCoordinates.y + 1, out var newPosition3))
                _validPositions.Add(newPosition3);
            if (_grid.TryGetPositionAt(selectedCoordinates.x - 1, selectedCoordinates.y, out var newPosition4))
                _validPositions.Add(newPosition4);
            if (_grid.TryGetPositionAt(selectedCoordinates.x, selectedCoordinates.y - 1, out var newPosition5))
                _validPositions.Add(newPosition5);

            return this;
        }


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

        public delegate bool Validator(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character, TPosition toPosition);

        public List<TPosition> CollectValidPositions()
        {
            return _validPositions;
        }

        public static bool Empty(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character, TPosition toPosition)
            => !board.TryGetPiece(toPosition, out var _);
        public static bool ContainsEnemy(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character, TPosition toPosition)
           => board.TryGetPiece(toPosition, out var toPiece) && toPiece.PlayerID != character.PlayerID;
    }
}