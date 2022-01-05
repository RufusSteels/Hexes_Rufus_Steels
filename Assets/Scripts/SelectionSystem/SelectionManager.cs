using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAE.SelectionSystem
{    public class SelectionEventArgs<TSelectableItem> : EventArgs
    {
        public TSelectableItem SelectableItem { get; }

        public SelectionEventArgs(TSelectableItem selectableItem)
        {
            SelectableItem = selectableItem;
        }
    }

    public interface ISelectableItem 
    {
        void Select();
        void Deselect();
    }

    public class SelectionManager<TSelectableItem> 
 //       where TSelectableItem : class/*moet een klasse zijn*/, ISelectableItem /* new() // zorgt voor een verplichte default constructor*/
    {
        public event EventHandler<SelectionEventArgs<TSelectableItem>> Selected;
        public event EventHandler<SelectionEventArgs<TSelectableItem>> Deselected;

        private HashSet<TSelectableItem> _selectableItems = new HashSet<TSelectableItem>();
        public HashSet<TSelectableItem> SelectableItems => _selectableItems;
        public TSelectableItem SelectableItem => _selectableItems.First();
        public bool HasSelection 
        { 
            get
            {
                return _selectableItems.Count > 0;
            } 
        }

        public bool IsSelected(TSelectableItem selectableItem)
            => _selectableItems.Contains(selectableItem);

        public bool Select(TSelectableItem selectableItem)
        {
            if (_selectableItems.Add(selectableItem))
            {
                OnSelected(new SelectionEventArgs<TSelectableItem>(selectableItem));
            }
            return false;
        }
        public bool Deselect(TSelectableItem selectableItem)
        {
            if (_selectableItems.Remove(selectableItem))
            {
                OnDeselected(new SelectionEventArgs<TSelectableItem>(selectableItem));
            }
            return false;
        }


        //bool van deze methode is nie success of fail, wel selected of deselected -> daarom commentaar
        public bool Toggle(TSelectableItem selectableItem)
        {
            if (IsSelected(selectableItem))
            {
                return !Deselect(selectableItem);
            }
            else
            {
                return Select(selectableItem);
            }
        }
        public void DeselectAll()
        {
            while(_selectableItems.Count > 0)
            {
                Deselect(_selectableItems.First());
            }

            foreach (TSelectableItem item in _selectableItems)
            {
                Deselect(item);
            }
        }

        protected virtual void OnSelected(SelectionEventArgs<TSelectableItem> e)
        {
            var handler = Selected;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDeselected(SelectionEventArgs<TSelectableItem> e)
        {
            var handler = Deselected;
            handler?.Invoke(this, e);
        }
    }
}
