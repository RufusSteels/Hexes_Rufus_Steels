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
        private BoardManager _boardManager;
        [SerializeField]
        private CardManager _cardManager;

        private Board<Character<Tile>, Tile> _board;
        private Grid<Tile> _grid;
        private SelectionManager<Character<Tile>> _selectionManager;
        private MoveManager<Tile> _moveManager;

        private int _currentPlayerID;
        private CharacterView _currentPlayer;
        private int _playerAmount = 1;


        // Start is called before the first frame update
        void Start()
        {
            _board = new Board<Character<Tile>, Tile>();
            _board.PieceMoved += (s, e) =>
            {
                _currentPlayerID = (_currentPlayerID + 1) % _playerAmount;
                SetCurrentPlayer();
            };
            _board.PiecePlaced += (s, e) => SetCurrentPlayer();

            _grid = new Grid<Tile>(2 * _boardManager.Distance + 1, 2 * _boardManager.Distance + 1);

            _moveManager = new MoveManager<Tile>(_board, _grid);

            _selectionManager = new SelectionManager<Character<Tile>>();
            _selectionManager.Selected += (s, e) =>
            {
                //e.SelectableItem.Activate = true;
                var tiles = _moveManager.ValidPositionsFor(e.SelectableItem, CardView.CurrentCard.CardType);
                foreach (var tile in tiles)
                {
                    tile.Highlight = true;
                }
            };
            _selectionManager.Deselected += (s, e) =>
            {
                //e.SelectableItem.Activate = false;
                var tiles = _moveManager.ValidPositionsFor(e.SelectableItem, CardView.CurrentCard.CardType);
                foreach (var tile in tiles)
                {
                    tile.Highlight = false;
                }
            };

            _boardManager.ResetBoard();
            _cardManager.CreateHand();

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
                tile.Dropped += (s, e) => Select(e.Tile);
                tile.DragEntered += (s, e) => 
                {
                    var validPositions = _moveManager.ValidPositionsFor(_currentPlayer.Model, CardView.CurrentCard.CardType);
                    foreach (Tile tile in validPositions)
                    {
                        tile.Highlight = false;
                    }
                    if (validPositions.Contains(tile))
                    {
                        HighlightAffectedTiles(_currentPlayer.Model, tile);
                    }
                    else
                    {
                        HighlightDroppableTiles(_currentPlayer.Model);
                    }
                };
                tile.DragExited += (s, e) =>
                {
                    var validPositions = _moveManager.ValidPositionsFor(_currentPlayer.Model, CardView.CurrentCard.CardType);
                    foreach (Tile tile in validPositions)
                    {
                        tile.Highlight = false;
                    }
                };
                _grid.Register(tile, q + _grid.Columns / 2, r + _grid.Rows / 2);
            }
        }

        private void ConnectPiece()
        {
            CharacterView[] characterViews = FindObjectsOfType<CharacterView>();
            foreach (CharacterView characterView in characterViews)
            {
                var character = new Character<Tile>();
                character.PlayerID = characterView.PlayerID;
                
                characterView.Model = character;

                var (q, r) = _hexPositionHelper.WorldToAxialPosition(/*_board, _grid,*/ characterView.transform.localPosition);

                if(_grid.TryGetPositionAt(q + _grid.Columns / 2, r + _grid.Rows / 2, out Tile tile))
                {
                    _board.Place(character, tile);
                }
            }
        }

        private void ConnectCards()
        {
            CardView[] cards = FindObjectsOfType<CardView>();
            foreach(CardView card in cards)
            {
                card.BeganDrag  += (s, e) => HighlightDroppableTiles(e.Character);
                //card.Dragged    += (s, e) => new NotImplementedException();
                card.EndedDrag += (s, e) => DeselectAll();
                //card.Dropped    += (s, e) => new NotImplementedException();
                //card.Dropped += (s, e) => DeselectAll();
            }
        }

        private void SetCurrentPlayer()
        {
            var characters = FindObjectsOfType<CharacterView>();
            foreach (CharacterView character in characters)
            {
                if (character.PlayerID == _currentPlayerID)
                {
                    _currentPlayer = character;
                }
            }
        }

        public void Select(Tile tile)
        {
            if (_board.TryGetPiece(tile, out var character) && character.PlayerID == _currentPlayerID)
            {
                Select(character);
            }
            else
            {
                if (_selectionManager.HasSelection)
                {
                    var selectedPiece = _selectionManager.SelectableItem;
                    _selectionManager.Deselect(selectedPiece);
            
                    var validPositions = _moveManager.ValidPositionsFor(selectedPiece, CardView.CurrentCard.CardType);
                    if (validPositions.Contains(tile))
                    {
                        _selectionManager.DeselectAll();
                        _moveManager.Execute(selectedPiece, tile, CardView.CurrentCard.CardType);
                        _cardManager.CycleCard();
                    }
                }
            }
        }

        public void Select(Character<Tile> character)
        {
           if (character.PlayerID == _currentPlayerID)
           {
               _selectionManager.DeselectAll();
               _selectionManager.Select(character);
           }
           else
           {
               if (_board.TryGetPosition(character, out var tile)) 
               {
                   Select(tile);
               }
           }
        }

        public void HighlightDroppableTiles(Character<Tile> character)
        {
            Select(character);
        }
        
        public void HighlightAffectedTiles(Character<Tile> character, Tile currentTile)
        {
            var affectedTiles = _moveManager.AffectedPositionsFor(character, currentTile, CardView.CurrentCard.CardType);
            foreach (Tile tile in affectedTiles)
            {
                tile.Highlight = true;
            }
        }

        public void DeselectAll()
            => _selectionManager.DeselectAll();
    }
}