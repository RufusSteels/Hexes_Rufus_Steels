using DAE.BoardSystem;
using DAE.ReplaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    class MovePush<TPosition> : MoveBase<TPosition>
        where TPosition : IPosition
    {
        public MovePush(Board<Character<TPosition>, TPosition> board, Grid<TPosition> grid, ReplayManager replayManager) : base(board, grid, replayManager)
        {

        }

        public override void Execute(Character<TPosition> character, List<TPosition> positions)
        {
            var enemies = new List<Character<TPosition>>();
            var oldPositions = new List<TPosition>();
            var taken = new List<bool>();

            Action forward = () =>
            {
                foreach (TPosition position in positions)
                {
                    if (Board.TryGetPiece(position, out Character<TPosition> enemyCharacter))
                    {
                        var toPosition = new HexMovementHelper<TPosition>(Board, Grid, character)
                        .Push(position)
                        .CollectValidPositions();

                        enemies.Add(enemyCharacter);
                        oldPositions.Add(position);
                        taken.Add(false);

                        if (toPosition.Count > 0)
                            Board.Move(enemyCharacter, toPosition[0]);
                        else
                        {
                            taken[taken.Count - 1] = true;
                            Board.Take(enemyCharacter);
                        }
                    }
                }
            };

            Action backward = () =>
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (!taken[i])
                    {
                        Board.Move(enemies[i], oldPositions[i]);
                    }
                    else
                    {
                        Board.Place(enemies[i], oldPositions[i]);
                    }
                }
            };

            ReplayManager.Execute(new DelegateReplayCommand(forward, backward));
        }

        public override List<TPosition> ValidPositions(Character<TPosition> character)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .TopRight(1)
                        .Right(1)
                        .BottomRight(1)
                        .BottomLeft(1)
                        .Left(1)
                        .TopLeft(1)
                        .CollectValidPositions();
        }

        public override List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position)
        {
            return new HexMovementHelper<TPosition>(Board, Grid, character)
                        .Rotate(position)
                        .CollectValidPositions();
        }

    }
}
