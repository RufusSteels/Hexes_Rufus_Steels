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

            _moves.Add(CardType.Teleport, new MoveTeleport<TPosition>(board, grid, replayManager));
            _moves.Add(CardType.Pushback, new MovePush<TPosition>(board, grid, replayManager));
            _moves.Add(CardType.Swipe, new MoveSwipe<TPosition>(board, grid, replayManager));
            _moves.Add(CardType.Slash, new MoveSlash<TPosition>(board, grid, replayManager));
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
        }

        public void Execute(Character<TPosition> character, TPosition position, CardType cardType)
        {
            var move = _moves[cardType]
                .Where(m => m.CanExecute(character))
                .Where(m => m.ValidPositions(character).Contains(position))
                .First();

            var positions = AffectedPositionsFor(character, position, cardType);
            move.Execute(character, positions);
        }
    }
}
