// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sekai.Framework.Structures;

public class WeakList<T> : IList, IList<T>, IReadOnlyList<T>, ICollection, ICollection<T>, IReadOnlyCollection<T>, IEnumerable, IEnumerable<T>, IEnumerator<T>, IDisposable
    where T : class
{
    /// <summary>
    ///     The backing object is just a list of weak references
    /// </summary>
    private readonly List<WeakReference<T>> list = new();

    #region IList
    /// <summary>
    ///     True if the list is fixed in size (it is not)
    /// </summary>
    public bool IsFixedSize => false;

    /// <summary>
    ///     True if the list is ReadOnly (it is not)
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    ///     Adds an object to the list
    /// </summary>
    /// <param name="value">The object to add to the list</param>
    /// <returns>The number of objects added</returns>
    public int Add(object? value)
    {
        if (value is not T v)
            return 0;

        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (this[i] == v) { return 0; }
                i++;
            }
            list.Add(v);
            return 1;
        }
    }

    /// <summary>
    ///     Gets or sets an object by index
    /// </summary>
    /// <param name="index">A non-negative integer index</param>
    /// <returns>A value, null otherwise</returns>
    object? IList.this[int index]
    {
        get => (index < 0 || index >= list.Count) ? null : list[index].GetValue();
        set
        {
            if (index < 0 || index >= list.Count)
                return;

            if (value is T v)
                list[index].SetTarget(v);
        }
    }


    /// <summary>
    ///     Clears the list
    /// </summary>
    public void Clear()
    {
        lock (SyncRoot)
        {
            list.Clear();
        }
    }

    /// <summary>
    ///     Checks if the list contains an item
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>true if the list contains the item</returns>
    public bool Contains(object? value)
    {
        if (value is not T v)
            return false;

        return Contains(v);
    }

    /// <summary>
    ///     Finds the index of an item
    /// </summary>
    /// <param name="item">The item to find</param>
    /// <returns>The index or -1</returns>
    public int IndexOf(object? value)
    {
        if (value is not T v)
            return -1;

        return IndexOf(v);
    }

    /// <summary>
    ///     Inserts an item at a given index
    /// </summary>
    /// <param name="index">The index to insert at</param>
    /// <param name="value">The value to insert</param>
    public void Insert(int index, object? value)
    {
        if (value is not T v)
            return;

        lock (SyncRoot)
        {
            list.Insert(index, new WeakReference<T>(v));
        }
    }


    /// <summary>
    ///     Removes a value from the list
    /// </summary>
    /// <param name="value"></param>
    public void Remove(object? value)
    {
        if (value != null) Remove((T)value);
    }

    /// <summary>
    ///     Removes an item at an index
    /// </summary>
    /// <param name="index">A non-negative integer giving the index of the item to remove</param>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= list.Count) { return; }
        lock (SyncRoot)
        {
            list.RemoveAt(index);
        }
    }
    #endregion IList

    #region IList<T>
    /// <summary>
    ///     Gets or sets an object by index
    /// </summary>
    /// <param name="index">A non-negative integer index</param>
    /// <returns>A value, null otherwise</returns>
    public T this[int index]
    {
#pragma warning disable CS8603
        get => (index < 0 || index >= list.Count) ? default : list[index].GetValue();
#pragma warning restore CS8603
        set
        {
            if (index < 0 || index >= list.Count)
                return;

            list[index].SetTarget(value);
        }
    }


    /// <summary>
    ///     Finds the index of an item
    /// </summary>
    /// <param name="item">The item to find</param>
    /// <returns>The index or -1</returns>
    public int IndexOf(T item)
    {
        if (item == null)
            return -1;

        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                T current = this[i];
                if (current == item) { return i; }
                if (current == null)
                {
                    list.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }
        return -1;
    }



    /// <summary>
    ///     Inserts an item at a given index
    /// </summary>
    /// <param name="index">The index to insert at</param>
    /// <param name="item">The item to insert</param>
    public void Insert(int index, T item)
    {
        if (item is not T v)
            return;

        lock (SyncRoot)
        {
            list.Insert(index, new WeakReference<T>(v));
        }
    }
    #endregion

    #region ICollection
    /// <summary>
    ///     Gets the number of items currently in the list
    /// </summary>
    public int Count => list.Count;

    /// <summary>
    ///     True if the list uses locks to synchronize itself in multi-threaded environments
    /// </summary>
    public bool IsSynchronized => true;

    /// <summary>
    ///     The object to lock when performing multi-threaded operations
    /// </summary>
    public object SyncRoot { get; private set; } = new object();

    /// <summary>
    ///     Copies the contents into an array
    /// </summary>
    /// <param name="array">The array to copy to</param>
    /// <param name="index">The index in the array to start copying at</param>
    public void CopyTo(Array array, int index)
    {
        lock (SyncRoot)
        {
            lock (array.SyncRoot)
            {
                int i = 0;
                while (i < list.Count && i + index < array.Length)
                {
                    var v = this[i];
                    while (v == null && i < list.Count)
                    {
                        list.RemoveAt(i);
                        v = this[i];
                    }
                    if (v != null)
                    {
                        array.SetValue(v, i + index);
                    }
                    i++;
                }
            }
        }
    }
    #endregion

    #region  ICollection<T>
    /// <summary>
    ///     Adds an object to the list
    /// </summary>
    /// <param name="value">The object to add to the list</param>
    public void Add(T item)
    {
        if (item is not T v)
            return;

        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (this[i] == v) { return; }
                i++;
            }
            list.Add(v);
        }
    }

    /// <summary>
    ///     Checks if the list contains an item
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>true if the list contains the item</returns>
    public bool Contains(T item)
    {
        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                T current = this[i];
                if (current == item)
                {
                    return true;
                }
                else if (current == null)
                {
                    list.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }
        return false;
    }


    /// <summary>
    ///     Copies the contents into an array
    /// </summary>
    /// <param name="array">The array to copy to</param>
    /// <param name="index">The index in the array at which to start copying</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count && i + arrayIndex < array.Length)
            {
                var v = this[i];
                while (v == null && i < list.Count)
                {
                    list.RemoveAt(i);
                    v = this[i];
                }
                if (v != null)
                {
                    array[i + arrayIndex] = v;
                }
                i++;
            }
        }
    }

    /// <summary>
    ///     Removes an item
    /// </summary>
    /// <param name="item">An item to remove</param>
    /// <returns>True if tiem removed</returns>
    public bool Remove(T item)
    {
        bool ret = false;
        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                var current = this[i];
                if (current == item || current == null)
                {
                    list.RemoveAt(i);
                    ret = ret || current != null;
                    i--;
                }
                i++;
            }
        }

        return false;
    }
    #endregion

    #region IEnumerator

    private int position = -1;

    /// <summary>
    ///     Gets the current object of the enumerator
    /// </summary>
    public object? Current
    {
        get
        {
            try
            {
                return list[position].GetValue();
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    ///     Gets the current value of the enumerator
    /// </summary>
    T? IEnumerator<T>.Current
    {
#pragma warning disable CS8768
        get
#pragma warning restore CS8768
        {
            try
            {
                return list[position].GetValue();
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    ///     Moves the enumerator to the next item
    /// </summary>
    /// <returns>True if the enumerator can move</returns>
    public bool MoveNext()
    {
        if (list == null) { return false; }
        position++;
        return position < list.Count;
    }

    /// <summary>
    ///     Resets the enumerator
    /// </summary>
    public void Reset()
    {
        lock (SyncRoot)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (this[i] == null)
                {
                    list.RemoveAt(i);
                    i--;
                }
                i++;
            }
            position = -1;
        }
    (list.GetEnumerator() as IEnumerator)?.Reset();
    }

    #endregion

    #region IEnumerable
    /// <summary>
    ///     This list is an enumerator
    /// </summary>
    /// <returns>The enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }
    #endregion

    #region IEnumerable<T>
    /// <summary>
    ///     Gets this object as an enumerator
    /// </summary>
    /// <returns>The enumerator</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }
    #endregion

    #region IDisposable
    /// <summary>
    ///     The dispose method of the list
    /// </summary>
    public void Dispose()
    {
        position = -1;
        GC.SuppressFinalize(this);
    }
    #endregion
}
