using DAE.CardSystem;
using DAE.ReplaySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DAE.GameSystem
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField]
        private int _deckSize = 13;
        [SerializeField]
        private int _handSize = 5;
        [SerializeField]
        private GameObject _card;
        [SerializeField]
        private Sprite[] _images;
        [SerializeField]
        private Transform _deck;
        [SerializeField]
        private Transform _hand;
        [SerializeField]
        private Transform _discard;
        public ReplayManager ReplayManager { get; set; }

        private void OnValidate()
        {
            if (_deckSize < _handSize)
                _deckSize = _handSize;
        }

        public void CreateHand()
        {
            for (int i = 0; i < _deckSize; i++)
            {
                var current = Instantiate(_card, _deck);
                if (current.TryGetComponent<CardView>(out CardView card))
                {
                    int type = UnityEngine.Random.Range(0, 4);
                    card.CardType = (CardType)type;
                    if(current.TryGetComponent<Image>(out Image image))
                    {
                        image.sprite = _images[type];
                    }
                }
            }

            for (int i = 0; i < _handSize; i++)
            {
                _deck.GetChild(0).SetParent(_hand);
            }
        }

        public void CycleCard()
        {
            Transform discarded = null;
            Transform drawn = null;

            Action forward = () =>
            {
                discarded = CardView.CurrentCard.transform;
                discarded.SetParent(_discard);

                if (_deck.childCount > 0)
                {
                    drawn = _deck.GetChild(0);
                    drawn.SetParent(_hand);
                }
            };

            Action backward = () =>
            {
                discarded.SetParent(_hand);
                drawn.SetParent(_deck);
            };

            ReplayManager.Append(forward, backward);
        }
    }
}