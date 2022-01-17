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

    public class Tile : MonoBehaviour, IPosition, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event EventHandler<TileEventArgs> Dropped;
        public event EventHandler<TileEventArgs> DragEntered;
        public event EventHandler<TileEventArgs> DragExited;

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

        public void OnDrop(PointerEventData eventData)
        {
            //Debug.Log("dropped");

            var handler = Dropped;
            Dropped?.Invoke(this, new TileEventArgs(this));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                //Debug.Log("DragEnter");

                var handler = DragEntered;
                DragEntered?.Invoke(this, new TileEventArgs(this));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                //Debug.Log("DragExit");

                var handler = DragExited;
                DragExited?.Invoke(this, new TileEventArgs(this));
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}