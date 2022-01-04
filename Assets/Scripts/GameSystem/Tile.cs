using DAE.HexSystem;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DAE.GameSystem
{
    public class TileEventArgs : EventArgs
    {
        public Tile Tile;
        public TileEventArgs(Tile tile)
        {
            Tile = tile;
        }
    }

    public class Tile : MonoBehaviour, IPointerClickHandler, IPosition
    {
        public event EventHandler<TileEventArgs> Clicked;

        [SerializeField]
        private UnityEvent OnActivate;
        [SerializeField]
        private UnityEvent OnDeactivate;

        public bool Highlight 
        {
            set
            {
                if (value)
                    OnActivate.Invoke();
                else
                    OnDeactivate.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked(new TileEventArgs(this));
        }

        protected virtual void OnClicked(TileEventArgs e)
        {
            var handler = Clicked;
            handler?.Invoke(this, e);
        }
    }
}