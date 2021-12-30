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
        public ClickEventArgs(Character<Tile> piece)
        {
            Character = piece;
        }
    }

    public class CharacterView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private PieceType _pieceType;

        [SerializeField]
        private int _playerID;

        public PieceType PieceType => _pieceType;
        public int PlayerID => _playerID;

        //event betekent dat callback niet van buitenaf kan opgeroepen worden
        //zorgt ervoor dat ge hem niet kunt overschrijven, enkel specifieke zaken bijvoegen of aftrekken
        //staat altijd bij een delegate

        //public event Action<Character> CallBack;

        //geen public of private... dan internal -> public in dezelfde assembly
        [SerializeField]
        private UnityEvent<bool> OnHighlight;
        private Character<Tile> model;

        public event EventHandler<ClickEventArgs> Clicked;

        public Character<Tile> Model 
        { 
            get => model;
            set
            {
                if (model != null)
                {
                    //model.ActivationStatusChanged -= OnCharacterActivationChanged;
                    model.Placed -= OnCharacterPlaced;
                    model.Taken -= OnCharacterTaken;
                    model.Moved -= OnCharacterMoved;
                }

                model = value;

                if (model != null)
                {
                    //model.ActivationStatusChanged += OnCharacterActivationChanged;
                    model.Placed += OnCharacterPlaced;
                    model.Taken += OnCharacterTaken;
                    model.Moved += OnCharacterMoved;
                }
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

        public bool Highlight
        {
            set
            {
                OnHighlight.Invoke(!value);
            }
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

        public override string ToString()
        {

            return "stukje lololololololololololololololololololololololol";
        }

        //public void OnCharacterActivationChanged(object source, CharacterEventArgs<Tile> eventArgs)
        //{
        //    Debug.Log("Activated " + eventArgs.Status);
        //}
    }
}
