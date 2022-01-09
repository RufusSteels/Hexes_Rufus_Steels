using DAE.Commons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAE.BoardSystem
{
    public class PiecePlacedEventArgs<TCharacter, TPosition> : EventArgs
    {
        public TPosition AtPosition { get; }
        public TCharacter Character { get; }

        public PiecePlacedEventArgs(TCharacter character, TPosition atPosition)
        {
            AtPosition = atPosition;
            Character = character;
        }
    }

    public class PieceMovedEventArgs<TCharacter, TPosition>
    {
        public TCharacter Character { get; }
        public TPosition FromPosition { get; }
        public TPosition ToPosition { get; }

        public PieceMovedEventArgs(TCharacter character, TPosition fromPosition, TPosition toPosition)
        {
            Character = character;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }
    }

    public class PieceTakenEventArgs<TCharacter, TPosition>
    {
        public TCharacter Character { get; }
        public TPosition Position { get; }

        public PieceTakenEventArgs(TCharacter character, TPosition position)
        {
            Character = character;
            Position = position;
        }
    }

    public class Board<TCharacter, TPosition>
    {
        public event EventHandler<PiecePlacedEventArgs<TCharacter, TPosition>> PiecePlaced;
        public event EventHandler<PieceMovedEventArgs<TCharacter, TPosition>> PieceMoved;
        public event EventHandler<PieceTakenEventArgs<TCharacter, TPosition>> PieceTaken;

        private BidirectionalDictionary<TCharacter, TPosition> _pieces
            = new BidirectionalDictionary<TCharacter, TPosition>();


        public bool TryGetPiece(TPosition position, out TCharacter character)
            => _pieces.TryGetKey(position, out character);

        public bool TryGetPosition(TCharacter character, out TPosition position)
            => _pieces.TryGetValue(character, out position);

        public void Move(TCharacter character, TPosition toPosition)
        {
            if (!TryGetPosition(character, out var fromPosition))
                return;

            if (TryGetPiece(toPosition, out _))
                return;

            if (!_pieces.Remove(character))
                return;

            _pieces.Add(character, toPosition);
            OnPieceMoved(new PieceMovedEventArgs<TCharacter, TPosition>(character, fromPosition, toPosition));
        }

        public void Place(TCharacter character, TPosition position)
        {
            if (_pieces.ContainsKey(character))
                return;

            if (_pieces.ContainsValue(position))
                return;

            _pieces.Add(character, position);

            OnPiecePlaced(new PiecePlacedEventArgs<TCharacter, TPosition>(character, position));
        }

        public void Take(TCharacter character)
        {
            if (!TryGetPosition(character, out TPosition fromPosition)) return;

            if (_pieces.Remove(character))
                OnPieceTaken(new PieceTakenEventArgs<TCharacter, TPosition>(character, fromPosition));
        }

        protected virtual void OnPiecePlaced(PiecePlacedEventArgs<TCharacter, TPosition> eventArgs)
        {
            var handler = PiecePlaced;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnPieceMoved(PieceMovedEventArgs<TCharacter, TPosition> eventArgs)
        {
            var handler = PieceMoved;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnPieceTaken(PieceTakenEventArgs<TCharacter, TPosition> eventArgs)
        {
            var handler = PieceTaken;
            handler?.Invoke(this, eventArgs);
        }
    }
}
