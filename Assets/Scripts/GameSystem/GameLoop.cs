using DAE.BoardSystem;
using DAE.CardSystem;
using DAE.HexSystem;
using DAE.ReplaySystem;
using DAE.SelectionSystem;
using DAE.StateSystem;
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
        private BoardManager _boardManager;
        [SerializeField]
        private CardManager _cardManager;
        private MoveManager<Tile> _moveManager;

        private StateMachine<GameStateBase> _gameStateMachine;

        [SerializeField]
        private Screen _startScreen;
        [SerializeField]
        private Screen _playScreen;
        [SerializeField]
        private Screen _endScreen;


        // Start is called before the first frame update
        void Start()
        {
            var board = new Board<Character<Tile>, Tile>();
            var grid = new Grid<Tile>(2 * _boardManager.Distance + 1, 2 * _boardManager.Distance + 1);
            var replayManager = new ReplayManager();
            _moveManager = new MoveManager<Tile>(board, grid, replayManager);
            _cardManager.ReplayManager = replayManager;

            _boardManager.ResetBoard();
            _cardManager.CreateHand();

            ConnectTile(grid);
            ConnectPiece(board, grid);
            ConnectCards();

            _gameStateMachine = new StateMachine<GameStateBase>();
            _gameStateMachine.Register(GameStateBase.PlayingState, new GameStatePlaying(_gameStateMachine, board, grid, replayManager, _cardManager));
            _gameStateMachine.Register(GameStateBase.ReplayingState, new GameStateReplay(_gameStateMachine, replayManager));
            _gameStateMachine.Register(GameStateBase.StartState, new GameStateStart(_gameStateMachine, _startScreen, _playScreen));
            _gameStateMachine.Register(GameStateBase.EndState, new GameStateEnd(_gameStateMachine, _endScreen, _playScreen));
            _gameStateMachine.InitialState = GameStateBase.StartState;

        }

        private void ConnectTile(Grid<Tile> grid)
        {
            var tiles = FindObjectsOfType<Tile>();
            foreach (Tile tile in tiles)
            {
                var (q, r) = _hexPositionHelper.WorldToAxialPosition(tile.transform.localPosition);
                tile.Dropped += (s, e) => Select(e.Tile);
                tile.DragEntered += (s, e) => 
                {
                    var validPositions = _moveManager.ValidPositionsFor(CharacterView.CurrentPlayer.Model, CardView.CurrentCard.CardType);
                    foreach (Tile tile in validPositions)
                    {
                        tile.Highlight = false;
                    }
                    if (validPositions.Contains(tile))
                    {
                        HighlightAffectedTiles(CharacterView.CurrentPlayer.Model, tile);
                    }
                    else
                    {
                        HighlightDroppableTiles(CharacterView.CurrentPlayer.Model);
                    }
                };
                tile.DragExited += (s, e) =>
                {
                    var validPositions = _moveManager.ValidPositionsFor(CharacterView.CurrentPlayer.Model, CardView.CurrentCard.CardType);
                    foreach (Tile tile in validPositions)
                    {
                        tile.Highlight = false;
                    }
                };
                grid.Register(tile, q + grid.Columns / 2, r + grid.Rows / 2);
            }
        }

        private void ConnectPiece(Board<Character<Tile>, Tile> board, Grid<Tile> grid)
        {
            CharacterView[] characterViews = FindObjectsOfType<CharacterView>();
            foreach (CharacterView characterView in characterViews)
            {
                var character = new Character<Tile>();
                character.PlayerID = characterView.PlayerID;
                
                characterView.Model = character;

                var (q, r) = _hexPositionHelper.WorldToAxialPosition(/*_board, _grid,*/ characterView.transform.localPosition);

                if(grid.TryGetPositionAt(q + grid.Columns / 2, r + grid.Rows / 2, out Tile tile))
                {
                    board.Place(character, tile);
                }
            }
        }

        private void ConnectCards()
        {
            CardView[] cards = FindObjectsOfType<CardView>();
            foreach(CardView card in cards)
            {
                card.BeganDrag  += (s, e) => HighlightDroppableTiles(e.Character);
                card.EndedDrag += (s, e) => _gameStateMachine.CurrentState.DeselectAll();
            }
        }

        public void Select(Tile tile)
        {
            _gameStateMachine.CurrentState.Select(tile);
        }

        public void Select(Character<Tile> character)
        {
            _gameStateMachine.CurrentState.Select(character);
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

        public void Backward()
            => _gameStateMachine.CurrentState.Backward();

        public void Forward()
            => _gameStateMachine.CurrentState.Forward();
    }
}