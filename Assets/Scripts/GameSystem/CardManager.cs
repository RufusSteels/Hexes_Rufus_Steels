using DAE.CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private Transform _deck;
        [SerializeField]
        private Transform _hand;
        [SerializeField]
        private Transform _discard;

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
                    card.CardType = (CardType)UnityEngine.Random.Range(0, 4);
            }

            for (int i = 0; i < _handSize; i++)
            {
                _deck.GetChild(0).SetParent(_hand);
            }
        }

        public void CycleCard()
        {
            CardView.CurrentCard.transform.SetParent(_discard);

            if(_deck.childCount > 0)
            {
                _deck.GetChild(0).SetParent(_hand);
            }
        }
    }
}