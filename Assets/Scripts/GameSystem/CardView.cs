using DAE.CardSystem;
using DAE.HexSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DAE.GameSystem
{
    public class BeginDragEventArgs : EventArgs
    {
        public Character<Tile> Character;
        public CardType CardType;
        
        public BeginDragEventArgs(Character<Tile> character, CardType cardType)
        {
            Character = character;
            CardType = cardType;
        }
    }
    public class DragEventArgs : EventArgs
    {

    }
    public class EndDragEventArgs : EventArgs
    {

    }
    public class DropEventArgs : EventArgs
    {

    }

    [RequireComponent(typeof(Image))]
    public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        private GameObject _cardPrefab;
        private GameObject _cardPreview;
        private RectTransform _draggingPlane;

        [SerializeField]
        private CardType _cardType = CardType.Teleport;

        public CardType CardType
        {
            get
            {
                return _cardType;
            }
            set
            {
                _cardType = value;
            }
        }

        public event EventHandler<BeginDragEventArgs> BeganDrag;
        public event EventHandler<DragEventArgs> Dragged;
        public event EventHandler<EndDragEventArgs> EndedDrag;
        public event EventHandler<DropEventArgs> Dropped;

        public void OnBeginDrag(PointerEventData eventData)
        {
            var canvas = FindInParents<Canvas>(gameObject);
            if (canvas == null)
                return;

            _cardPreview = Instantiate(_cardPrefab, canvas.transform);

            _draggingPlane = canvas.transform as RectTransform;

            SetDraggedPosition(eventData);

            var characters = FindObjectsOfType<CharacterView>();
            Character<Tile> player = null;
            foreach (CharacterView view in characters)
            {
                if (view.PlayerID == 0)
                {
                    player = view.Model;
                }
            }
            var handler = BeganDrag;
            handler?.Invoke(this, new BeginDragEventArgs(player, _cardType));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_cardPreview != null)
                SetDraggedPosition(eventData);
        }

        private void SetDraggedPosition(PointerEventData data)
        {
            var rt = _cardPreview.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos;
                rt.rotation = _draggingPlane.rotation;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_cardPreview != null)
                Destroy(_cardPreview);

            var handler = EndedDrag;
            handler?.Invoke(this, new EndDragEventArgs());
        }

        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;
            var comp = go.GetComponent<T>();

            if (comp != null)
                return comp;

            Transform t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
            return comp;
        }
    }
}