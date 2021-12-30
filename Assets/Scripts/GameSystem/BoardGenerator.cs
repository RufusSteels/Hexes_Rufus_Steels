using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAE.GameSystem
{
    public class BoardGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _tile;

        [SerializeField]
        private GameObject _enemy;
        [SerializeField]
        private int _enemyAmount = 1;

        [SerializeField]
        private HexPositionHelper _hexPositionHelper;

        [SerializeField] [Range(1,10)]
        private int _distance;

        [SerializeField] [Range(0, 1)]
        private float _spawnOdds = .2f;

        public int Distance 
        { 
            get 
            { 
                return _distance; 
            } 
        }

        private void OnValidate()
        {
            if (_distance <= 0)
                _distance = 1;
        
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

        private void CreateEnemies()
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
            CreateEnemies();
        }
    }
}