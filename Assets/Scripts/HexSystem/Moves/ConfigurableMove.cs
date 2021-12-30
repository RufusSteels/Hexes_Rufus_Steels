using DAE.BoardSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    class ConfigurableMove<TPosition> : MoveBase<TPosition>
        where TPosition : IPosition
    {
        public delegate List<TPosition> PositionsCollector(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> piece);
        private PositionsCollector _collectPositions;

        public ConfigurableMove(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, PositionsCollector positionsCollector) : base(board, grid)
        {
            _collectPositions = positionsCollector;
        }

        public override List<TPosition> Positions(Character<TPosition> piece)
            => _collectPositions(Board, Grid, piece);
    }
}
