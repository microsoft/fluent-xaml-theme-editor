// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FluentEditorShared
{
    // This is intended to be used as a binding soruce for XAML ItemsControls not as a fully flexible and generic List
    // the constraint on T simplifies some logic and XAML would box any value types anyway
    // Not thread safe
    public class ObservableList<T> : INotifyCollectionChanged, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList where T : class
    {
        public ObservableList()
        {
            _list = new List<T>();
        }

        public ObservableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        private List<T> _list;

        private bool _sendNotifications = true;
        public bool SendNotifications
        {
            get { return _sendNotifications; }
            set { _sendNotifications = value; }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            var lastIndex = _list.Count;
            if (lastIndex > 0)
            {
                lastIndex--;
            }
            IList addedItems = collection.ToList();
            _list.AddRange(collection);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems, lastIndex));
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            IList addedItems = collection.ToList();
            _list.InsertRange(index, collection);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems, index));
        }

        public int RemoveAll(Predicate<T> match)
        {
            var removeList = _list.FindAll(match);
            var count = _list.RemoveAll(match);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removeList));
            return count;
        }

        public void MoveItem(T item, int targetIndex)
        {
            int oldIndex = _list.IndexOf(item);
            _list.Remove(item);
            if (targetIndex == _list.Count)
            {
                _list.Add(item);
            }
            else
            {
                _list.Insert(targetIndex, item);
            }
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, targetIndex, oldIndex));
        }

        public void MoveItemBefore(T item, T targetItem)
        {
            int oldIndex = _list.IndexOf(item);
            _list.Remove(item);
            int targetIndex = _list.IndexOf(targetItem);
            _list.Insert(targetIndex, item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, targetIndex, oldIndex));
        }

        public void MoveItemAfter(T item, T targetItem)
        {
            int oldIndex = _list.IndexOf(item);
            _list.Remove(item);
            int targetIndex = _list.IndexOf(targetItem) + 1;
            if (targetIndex == _list.Count)
            {
                _list.Add(item);
            }
            else
            {
                _list.Insert(targetIndex, item);
            }
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, targetIndex, oldIndex));
        }

        public void Sort(Comparison<T> comparison)
        {
            if (_list.Count == 0)
            {
                return;
            }
            _list.Sort(comparison);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, _list as IList, 0, 0));
        }

        public void Sort(IComparer<T> comparer)
        {
            if (_list.Count == 0)
            {
                return;
            }
            _list.Sort(comparer);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, _list as IList, 0, 0));
        }

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (_sendNotifications)
            {
                CollectionChanged?.Invoke(this, args);
            }
        }

        #endregion

        #region IList<T>

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                T oldItem = _list[index];
                _list[index] = value;
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
            }
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            var removedItem = _list[index];
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        #endregion

        #region IList

        object IList.this[int index]
        {
            get { return _list[index]; }
            set
            {
                T oldItem = _list[index];
                _list[index] = value as T;
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, _list[index], oldItem));
            }
        }

        public bool IsFixedSize
        {
            get { return (_list as IList).IsFixedSize; }
        }

        public int Add(object value)
        {
            var index = (_list as IList).Add(value);
            if (index >= 0)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            }
            return index;
        }

        public bool Contains(object value)
        {
            return (_list as IList).Contains(value);
        }

        public int IndexOf(object value)
        {
            return (_list as IList).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            if (value is T item)
            {
                _list.Insert(index, item);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void Remove(object value)
        {
            if (value is T item)
            {
                var removedIndex = _list.IndexOf(item);
                _list.Remove(item);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, removedIndex));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<T>

        public int Count { get { return _list.Count; } }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)_list).IsReadOnly;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);
            var index = _list.IndexOf(item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void Clear()
        {
            _list.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removedIndex = _list.IndexOf(item);
            var retVal = _list.Remove(item);
            if (retVal)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, removedIndex));
            }
            return retVal;
        }

        #endregion

        #region ICollection

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)_list).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)_list).SyncRoot;
            }
        }

        public void CopyTo(Array array, int index)
        {
            (_list as IList).CopyTo(array, index);
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
