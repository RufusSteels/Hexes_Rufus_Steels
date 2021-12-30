using DAE.BoardSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DAE.HexSystem
{
    internal class MovementHelper<TPosition>
        where TPosition : IPosition
    {
        private Board<Character<TPosition>, TPosition> _board;
        private Grid<TPosition> _grid;
        private Character<TPosition> _piece;
        private List<TPosition> _validPositions = new List<TPosition>();

        public MovementHelper(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece)
        {
            _board = board;
            _grid = grid;
            _piece = piece;
        }

        public MovementHelper<TPosition> North(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(0, 1, maxSteps, validators); //expressionBody één lijn code voor een methode kan ook zo geschreven worden
        public MovementHelper<TPosition> East(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(1, 0, maxSteps, validators);
        public MovementHelper<TPosition> South(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(0, -1, maxSteps, validators);
        public MovementHelper<TPosition> West(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(-1, 0, maxSteps, validators);

        public MovementHelper<TPosition> NorthEast(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(1, 1, maxSteps, validators);
        public MovementHelper<TPosition> SouthEast(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(1, -1, maxSteps, validators);
        public MovementHelper<TPosition> SouthWest(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(-1, -1, maxSteps, validators);
        public MovementHelper<TPosition> NorthWest(int maxSteps = int.MaxValue, params Validator[] validators)
            => Collect(-1, 1, maxSteps, validators);

        public MovementHelper<TPosition> KnightMoves(int maxSteps = int.MaxValue, params Validator[] validators)
        {
            Collect(2, 1, maxSteps, validators);
            Collect(2, -1, maxSteps, validators);
            Collect(-2, 1, maxSteps, validators);
            Collect(-2, -1, maxSteps, validators);

            Collect(1, 2, maxSteps, validators);
            Collect(1, -2, maxSteps, validators);
            Collect(-1, 2, maxSteps, validators);
            Collect(-1, -2, maxSteps, validators);

            return this;
        }

        public MovementHelper<TPosition> Collect(int xOffset, int yOffset, int maxSteps = int.MaxValue, params Validator[] validators)
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
                if (_board.TryGetPiece(nextPosition, out Character<TPosition> nextPiece))
                {
                    if (nextPiece.PlayerID != _piece.PlayerID)
                        _validPositions.Add(nextPosition);
                    break;
                }
                else
                {
                    _validPositions.Add(nextPosition);

                    nextCoordinateX += xOffset;
                    nextCoordinateY += yOffset;

                    _grid.TryGetPositionAt(
                        nextCoordinateX,
                        nextCoordinateY,
                        out nextPosition);
                    steps++;
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