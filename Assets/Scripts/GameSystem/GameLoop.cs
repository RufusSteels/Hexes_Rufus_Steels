using DAE.BoardSystem;
using DAE.CardSystem;
using DAE.HexSystem;
using DAE.SelectionSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAE.GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private HexPositionHelper _hexPositionHelper;

        [SerializeField]
        private Transform _boardParent;
        [SerializeField]
        private BoardGenerator _boardGenerator;

        private Board<Character<Tile>, Tile> _board;
        private Grid<Tile> _grid;
        private SelectionManager<Character<Tile>> _selectionManager;
        private MoveManager<Tile> _moveManager;

        private int _currentPlayerID;


        // Start is called before the first frame update
        void Start()
        {
            _board = new Board<Character<Tile>, Tile>();
            _board.PieceMoved += (s, e) =>
            {
                _currentPlayerID = (_currentPlayerID + 1) % 2;
            };

            _grid = new Grid<Tile>(2 * _boardGenerator.Distance + 1, 2 * _boardGenerator.Distance + 1);

            _moveManager = new MoveManager<Tile>(_board, _grid);

            _selectionManager = new SelectionManager<Character<Tile>>();
            _selectionManager.Selected += (s, e) =>
            {
                //e.SelectableItem.Activate = true;
                var tiles = _moveManager.ValidPositionsFor(e.SelectableItem);
                foreach (var tile in tiles)
                {
                    tile.Highlight = true;
                }
            };
            _selectionManager.Deselected += (s, e) =>
            {
                //e.SelectableItem.Activate = false;
                var tiles = _moveManager.ValidPositionsFor(e.SelectableItem);
                foreach (var tile in tiles)
                {
                    tile.Highlight = false;
                }
            };

            _boardGenerator.ResetBoard();

            ConnectTile();
            ConnectPiece();
            ConnectCards();
        }

        private void ConnectTile()
        {
            var tiles = FindObjectsOfType<Tile>();
            foreach (Tile tile in tiles)
            {
                var (q, r) = _hexPositionHelper.WorldToAxialPosition(/*_board, _grid, */tile.transform.localPosition);
                tile.Clicked += (s, e) => Select(e.Tile);
                _grid.Register(tile, q + _grid.Columns / 2, r + _grid.Rows / 2);

                //Debug.Log($"connected {tile}: ({q}, {r})");
                //tile.Highlight = true;
            }
            //Debug.Log("Loop Ended");
        }

        private void ConnectPiece()
        {
            CharacterView[] characterViews = FindObjectsOfType<CharacterView>();
            foreach (CharacterView characterView in characterViews)
            {
                var character = new Character<Tile>();
                //character.PieceType = characterView.PieceType;
                character.PlayerID = characterView.PlayerID;
                
                characterView.Model = character;

                var (q, r) = _hexPositionHelper.WorldToAxialPosition(/*_board, _grid,*/ characterView.transform.localPosition);

                if(_grid.TryGetPositionAt(q + _grid.Columns / 2, r + _grid.Rows / 2, out Tile tile))
                {
                    _board.Place(character, tile);
                }

                characterView.Clicked += (s, e) => Select(e.Character); 
            }
        }

        private void ConnectCards()
        {
            Card[] cards = FindObjectsOfType<Card>();
            foreach(Card card in cards)
            {
                card.BeganDrag  += (s, e) => new NotImplementedException();
                card.Dragged    += (s, e) => new NotImplementedException();
                card.EndedDrag  += (s, e) => new NotImplementedException();
                card.Dropped    += (s, e) => new NotImplementedException();
            }
        }

        public void Select(Tile tile)
        {
            if (_board.TryGetPiece(tile, out var piece) && piece.PlayerID == _currentPlayerID)
            {
                Select(piece);
            }
            else
            {
                if (_selectionManager.HasSelection)
                {
                    var selectedPiece = _selectionManager.SelectableItem;
                    _selectionManager.Deselect(selectedPiece);

                    var validPositions = _moveManager.ValidPositionsFor(selectedPiece);
                    if (validPositions.Contains(tile))
                    {
                        _selectionManager.DeselectAll();
                        _moveManager.Move(selectedPiece, tile);
                    }
                }
            }
        }

        public void Select(Character<Tile> piece)
        {
            if (piece.PlayerID == _currentPlayerID)
            {
                _selectionManager.DeselectAll();
                _selectionManager.Select(piece);
            }
            else
            {
                if (_board.TryGetPosition(piece, out var tile)) 
                {
                    Select(tile);
                }
            }
        }

        public void DeselectAll()
            => _selectionManager.DeselectAll();

        //public void DebugTile(Tile tile)
        //{
        //    var gridPos = _hexPositionHelper.WorldToGridPosition(_board, _grid, tile.transform.localPosition);
        //    var worldPos = _hexPositionHelper.GridToWorldPosition(_board, _grid, gridPos.x, gridPos.y);
        //
        //    Debug.Log($"Tile: {tile.name}, at GP {gridPos},  at WP{worldPos}");
        //}

        private void OnPieceClicked(Character<Tile> p) 
            => _selectionManager.Toggle(p);
    }
}