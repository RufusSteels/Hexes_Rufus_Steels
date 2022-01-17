
using DAE.BoardSystem;
using DAE.HexSystem;
using DAE.ReplaySystem;
using DAE.SelectionSystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DAE.GameSystem
{
    class GameStatePlaying : GameStateBase
    {
        private Board<Character<Tile>, Tile> _board;
        private Grid<Tile> _grid;

        private MoveManager<Tile> _moveManager;
        private SelectionManager<Character<Tile>> _selectionManager;
        private CardManager _cardManager;

        private int _currentPlayerID;
        private CharacterView _currentPlayer;
        private int _playerAmount = 1;

        public GameStatePlaying(StateMachine<GameStateBase> stateMachine, Board<Character<Tile>, Tile> board, Grid<Tile> grid, ReplayManager replayManager, CardManager cardManager) : base(stateMachine)
        {
            _board = board;
            _grid = grid;

            _moveManager = new MoveManager<Tile>(board, grid, replayManager);
            _selectionManager = new SelectionManager<Character<Tile>>();
            _cardManager = cardManager;
        }

        public override void OnEnter()
        {
            _selectionManager.Selected += OnPieceSelected;
            _selectionManager.Deselected += OnPieceDeselected;
            _board.PieceMoved += OnPieceMoved;
            _board.PieceTaken += OnPieceTaken;
        }

        public override void OnExit()
        {
            _selectionManager.Selected -= OnPieceSelected;
            _selectionManager.Deselected -= OnPieceDeselected;
            _board.PieceMoved -= OnPieceMoved;
        }

        public override void Select(Character<Tile> character)
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

        public override void Select(Tile tile)
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

        public override void Backward()
        {
            StateMachine.MoveTo(ReplayingState);
        }

        public override void DeselectAll()
            => _selectionManager.DeselectAll();

        private void OnPieceSelected(object source, SelectionEventArgs<Character<Tile>> eventArgs)
        {
            //e.SelectableItem.Activate = true;
            var tiles = _moveManager.ValidPositionsFor(eventArgs.SelectableItem, CardView.CurrentCard.CardType);
            foreach (var tile in tiles)
            {
                tile.Highlight = true;
            }
        }

        private void OnPieceDeselected(object source, SelectionEventArgs<Character<Tile>> eventArgs)
        {
            //e.SelectableItem.Activate = false;
            var tiles = _moveManager.ValidPositionsFor(eventArgs.SelectableItem, CardView.CurrentCard.CardType);
            foreach (var tile in tiles)
            {
                tile.Highlight = false;
            }
        }

        private void OnPieceMoved(object source, PieceMovedEventArgs<Character<Tile>, Tile> eventArgs)
        {
            _currentPlayerID = (_currentPlayerID + 1) % _playerAmount;
        }

        private void OnPieceTaken(object source, PieceTakenEventArgs<Character<Tile>, Tile> eventArgs)
        {
            if (eventArgs.Character.PlayerID == _currentPlayerID)
            {
                Debug.Log("Player Taken");
                StateMachine.MoveTo(GameStateBase.EndState);
            }
        }
    }
}
