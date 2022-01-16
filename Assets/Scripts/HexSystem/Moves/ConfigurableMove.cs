using DAE.BoardSystem;
using DAE.ReplaySystem;
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
        public delegate List<TPosition> PositionsCollector(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character);
        public delegate List<TPosition> AffectedPositionsCollector(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character, TPosition position);
        public delegate void PositionExecutionHandler(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, Character<TPosition> character, List<TPosition> position);

        private PositionsCollector _collectValidPositions;
        private AffectedPositionsCollector _collectAffectedPositions;
        private PositionExecutionHandler _executeAtPositions;


        public ConfigurableMove(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager, PositionsCollector positionsCollector, AffectedPositionsCollector affectedPositionsCollector, PositionExecutionHandler positionExecutionHandler) : base(board, grid, replayManager)
        {
            _collectValidPositions = positionsCollector;
            _collectAffectedPositions = affectedPositionsCollector;
            _executeAtPositions = positionExecutionHandler;
        }

        public override void Execute(Character<TPosition> character, List<TPosition> positions)
            => _executeAtPositions(Board, Grid, character, positions);

        public override List<TPosition> ValidPositions(Character<TPosition> character)
            => _collectValidPositions(Board, Grid, character);

        public override List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position)
            => _collectAffectedPositions(Board, Grid, character, position);
    }
}
