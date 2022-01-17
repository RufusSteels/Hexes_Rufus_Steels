using DAE.BoardSystem;
using DAE.ReplaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    class MoveBomb<TPosition> : MoveBase<TPosition>
        where TPosition : IPosition
    {
        public MoveBomb(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager) : base(board, grid, replayManager)
        {

        }

        public override void Execute(Character<TPosition> character, List<TPosition> positions)
        {
            var enemies = new List<Character<TPosition>>();
            var oldPositions = new List<TPosition>();
            var destroyedTiles = new List<TPosition>();


            Action forward = () =>
            {
                foreach (TPosition position in positions)
                {
                    if(Board.TryGetPiece(position, out var character))
                    {
                        enemies.Add(character);
                        oldPositions.Add(position);
                        Board.Take(character);
                    }
                    destroyedTiles.Add(position);
                    position.Disable();
                }
            };

            Action backward = () =>
            {
                foreach (TPosition position in destroyedTiles)
                {
                    position.Enable();
                }
                for (int i = 0; i < enemies.Count; i++)
                {
                    Board.Place(enemies[i], oldPositions[i]);
                }
            };

            ReplayManager.Execute(new DelegateReplayCommand(forward, backward));
        }

        public override List<TPosition> ValidPositions(Character<TPosition> character)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .All()
                        .CollectValidPositions();
        }

        public override List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .Surround(position)
                        .CollectValidPositions();
        }

    }
}
