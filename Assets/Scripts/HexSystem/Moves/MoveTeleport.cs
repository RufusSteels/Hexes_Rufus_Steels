using DAE.BoardSystem;
using DAE.ReplaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    class MoveTeleport<TPosition> : MoveBase<TPosition>
        where TPosition : IPosition
    {
        public MoveTeleport(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager) : base(board, grid, replayManager)
        {

        }

        public override void Execute(Character<TPosition> character, List<TPosition> positions)
        {
            if (!Board.TryGetPosition(character, out var oldPosition))
                return;

            Action forward = () =>
            {
                Board.Move(character, positions[0]);
            };

            Action backward = () =>
            {
                Board.Move(character, oldPosition);
            };

            ReplayManager.Execute(new DelegateReplayCommand(forward, backward));
        }

        public override List<TPosition> ValidPositions(Character<TPosition> character)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .All(HexMovementHelper<TPosition>.Empty)
                        .CollectValidPositions();
        }

        public override List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .Current(position)
                        .CollectValidPositions();
        }
    }
}
