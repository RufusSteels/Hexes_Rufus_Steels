using DAE.HexSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//[System.Diagnostics.DebuggerDisplay]

namespace DAE.GameSystem
{
    public class ClickEventArgs : EventArgs
    {
        public Character<Tile> Character { get; }
        public ClickEventArgs(Character<Tile> character)
        {
            Character = character;
        }
    }

    public class CharacterView : MonoBehaviour, IPointerClickHandler
    {
        public static CharacterView CurrentPlayer
        {
            get
            {
                var characters = FindObjectsOfType<CharacterView>();
                foreach (CharacterView character in characters)
                {
                    if (character.PlayerID == CurrentPlayerID)
                    {
                        return character;
                    }
                }
                return null;
            }
        }
        public static int CurrentPlayerID = 0;

        [SerializeField]
        private int _playerID;

        [SerializeField]
        private UnityEvent<bool> OnHighlight;
        private Character<Tile> _model;

        public event EventHandler<ClickEventArgs> Clicked;

        public int PlayerID => _playerID;
        public Character<Tile> Model 
        { 
            get => _model;
            set
            {
                if (_model != null)
                {
                    //model.ActivationStatusChanged -= OnCharacterActivationChanged;
                    _model.Placed -= OnCharacterPlaced;
                    _model.Taken -= OnCharacterTaken;
                    _model.Moved -= OnCharacterMoved;
                }
        
                _model = value;
        
                if (_model != null)
                {
                    //model.ActivationStatusChanged += OnCharacterActivationChanged;
                    _model.Placed += OnCharacterPlaced;
                    _model.Taken += OnCharacterTaken;
                    _model.Moved += OnCharacterMoved;
                }
            }
        }

        public bool Highlight
        {
            set
            {
                OnHighlight.Invoke(!value);
            }
        }

        private void OnCharacterPlaced(object sender, CharacterEventArgs<Tile> e)
        {
            gameObject.transform.position = e.Position.transform.position;
            gameObject.SetActive(true);
        }

        private void OnCharacterTaken(object sender, CharacterEventArgs<Tile> e)
        {
            gameObject.SetActive(false);
        }

        private void OnCharacterMoved(object sender, CharacterEventArgs<Tile> e)
        {
            gameObject.transform.position = e.Position.transform.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(this.gameObject.name);
        
            OnClicked(new ClickEventArgs(Model));
        }

        protected virtual void OnClicked(ClickEventArgs e)
        {
            var handler = Clicked;
            //if (handler != null)
            //{
            //    handler(this, EventArgs.Empty);
            //}
            //is hetzelfde als hieronder
            handler?.Invoke(this, e);
        }
    }
}
