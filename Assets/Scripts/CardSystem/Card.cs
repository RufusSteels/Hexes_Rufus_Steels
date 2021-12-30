using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DAE.CardSystem
{
    [RequireComponent(typeof(Image))]
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private GameObject _cardPrefab;
        private GameObject _cardPreview;
        private RectTransform _draggingPlane;

        public void OnBeginDrag(PointerEventData eventData)
        {
            var canvas = FindInParents<Canvas>(gameObject);
            if (canvas == null)
                return;

            _cardPreview = Instantiate(_cardPrefab, canvas.transform);

            _draggingPlane = canvas.transform as RectTransform;

            SetDraggedPosition(eventData);
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
