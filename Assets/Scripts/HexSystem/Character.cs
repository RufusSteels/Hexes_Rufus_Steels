using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    public class CharacterEventArgs<TPosition> : EventArgs
        where TPosition : IPosition
    {
        public TPosition Position { get; set; }

        public CharacterEventArgs(TPosition position)
        {
            Position = position;
        }
    }


    public class Character<TPosition>
        where TPosition : IPosition
    {
        public event EventHandler<CharacterEventArgs<TPosition>> Placed;
        public event EventHandler<CharacterEventArgs<TPosition>> Taken;
        public event EventHandler<CharacterEventArgs<TPosition>> Moved;

        internal bool HasMoved { get; set; }
        public PieceType PieceType{ get; set; }
        public int PlayerID { get; set; }

        public void PlaceAt(TPosition position)
        {
            OnPlaced(new CharacterEventArgs<TPosition>(position));
        }
        public void TakeFrom(TPosition position)
        {
            OnTaken(new CharacterEventArgs<TPosition>(position));
        }
        public void MoveTo(TPosition position)
        {
            OnMoved(new CharacterEventArgs<TPosition>(position));
        }

        protected virtual void OnPlaced(CharacterEventArgs<TPosition> e)
        {
            var handler = Placed;
            handler?.Invoke(this, e);
        }
        protected virtual void OnTaken(CharacterEventArgs<TPosition> e)
        {
            var handler = Taken;
            handler?.Invoke(this, e);
        }
        protected virtual void OnMoved(CharacterEventArgs<TPosition> e)
        {
            var handler = Moved;
            handler?.Invoke(this, e);
        }
    }
}
