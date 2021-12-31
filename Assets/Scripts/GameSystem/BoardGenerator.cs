using DAE.CardSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAE.GameSystem
{
    public class BoardGenerator : MonoBehaviour
    {
        [SerializeField]
        private HexPositionHelper _hexPositionHelper;

        [SerializeField]
        private GameObject _tile;
        [SerializeField] [Range(1, 10)]
        private int _distance;

        [SerializeField]
        private int _deckSize = 13;
        [SerializeField]
        private int _handSize = 5;
        [SerializeField]
        private Transform _deck;
        [SerializeField]
        private Transform _hand;

        [SerializeField]
        private GameObject _card;
        [SerializeField]
        private GameObject _player;
        [SerializeField]
        private GameObject _enemy;
        [SerializeField] [Range(0, 1)]
        private float _spawnOdds = .2f;
        //[SerializeField]
        //private int _enemyAmount = 1;


        public int Distance => _distance;

        private void OnValidate()
        {
            if (_distance <= 0)
                _distance = 1;

            if(_deckSize < _handSize)
                _deckSize = _handSize;
        
            //ClearBoard();
            //CreateBoard();
        }

        private void ClearBoard()
        {
            //foreach (Transform child in transform)
            //{
            //    StartCoroutine(Destroy(child.gameObject));
            //}
            //
            //IEnumerator Destroy(GameObject go)
            //{
            //    yield return null;
            //    DestroyImmediate(go);
            //}

            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void CreateBoard()
        {
            for (int q = -_distance; q <= _distance; q++)
            {

                for (int r = -_distance; r <= _distance; r++)
                {

                    for (int s = -_distance; s <= _distance; s++)
                    {
                        if (q + r + s == 0)
                        {
                            var tile = Instantiate(_tile, transform);
                            tile.transform.position = _hexPositionHelper.AxialToWorldPosition(q, r);
                            tile.transform.localScale = Vector3.one * _hexPositionHelper.TileSize;
                        }
                    }
                }
            }
        }

        private void CreateHand()
        {
            for (int i = 0; i < _deckSize; i++)
            {
                var current = Instantiate(_card, _deck);
                if(current.TryGetComponent<Card>(out Card card))
                    card.CardType = (CardType) UnityEngine.Random.Range(0, 4);
            }

            for (int i = 0; i < _handSize; i++)
            {
                _deck.GetChild(0).parent = _hand;
            }
        }

        private void CreateCharacters()
        {
            Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

            foreach (Tile tile in tiles)
            {
                if(UnityEngine.Random.value < _spawnOdds && _hexPositionHelper.WorldToAxialPosition(tile.transform.position) != (0, 0))
                {
                    //Debug.Log("enemy spawned");
                    var enemy = Instantiate(_enemy, gameObject.transform);
                    enemy.transform.localPosition = tile.gameObject.transform.localPosition;
                }

                if (_hexPositionHelper.WorldToAxialPosition(tile.transform.position) == (0, 0))
                {
                    var player = Instantiate(_player, gameObject.transform);
                    player.transform.localPosition = tile.gameObject.transform.localPosition;
                }
            }

            //if (_enemyAmount > tiles.Length - 1)
            //    _enemyAmount = tiles.Length - 1;
            //
            //for (int i = 0; i < _enemyAmount; i++)
            //{
            //    //tiles[0].
            //    Instantiate(_enemy, gameObject.transform);
            //}
        }

        public void ResetBoard()
        {
            ClearBoard();
            CreateBoard();
            CreateHand();
            CreateCharacters();
        }
    }
}