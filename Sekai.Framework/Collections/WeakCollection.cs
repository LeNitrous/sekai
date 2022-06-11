// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// The MIT License (MIT)

// Copyright (c) 2013 Thomas Ibel

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Framework.Collections;

/// <summary>
/// A collections which only holds weak references to the items.
/// <br></br>
/// This implementation is derived from <see href="https://github.com/tibel/Weakly">Weakly</see>.
/// </summary>
/// <typeparam name="T">The type of the elements in the collection.</typeparam>
internal class WeakCollection<T> : ICollection<T>
    where T : class
{
    private readonly List<WeakReference> inner;
    private readonly WeakReference gcm = new(new object());

    private void cleanIfNeeded()
    {
        if (gcm.IsAlive)
            return;

        gcm.Target = new object();
        Purge();
    }

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakCollection&lt;T&gt;"/> class that is empty and has the default initial capacity.
    /// </summary>
    public WeakCollection()
    {
        inner = new List<WeakReference>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakCollection&lt;T&gt;"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    public WeakCollection(IEnumerable<T> collection)
    {
        inner = new List<WeakReference>(collection.Select(item => new WeakReference(item)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakCollection&lt;T&gt;"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    public WeakCollection(int capacity)
    {
        inner = new List<WeakReference>(capacity);
    }

    #endregion

    /// <summary>
    /// Removes all dead entries.
    /// </summary>
    /// <returns>true if entries where removed; otherwise false.</returns>
    public bool Purge()
    {
        return inner.RemoveAll(l => !l.IsAlive) > 0;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    // Looks like we're getting nullability checks here, this is intended, so we're disabling the check here.
#pragma warning disable CS8600, CS8619
    public IEnumerator<T> GetEnumerator()
    {
        cleanIfNeeded();

        var enumerable = inner.Select(item => (T)item.Target)
            .Where(value => value is object);

        return enumerable.GetEnumerator();
    }
#pragma warning restore CS8600, CS8619

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(T item)
    {
        cleanIfNeeded();
        inner.Add(new WeakReference(item));
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear()
    {
        inner.Clear();
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
    public bool Contains(T item)
    {
        cleanIfNeeded();
        return inner.FindIndex(w => ((T?)w.Target) == item) >= 0;
    }

    /// <summary>
    /// Copies the elements of the collection to an Array, starting at a particular Array index.
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        cleanIfNeeded();

        if (array is null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if ((arrayIndex + inner.Count) > array.Length)
            throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");

        var items = inner.Select(item => (T?)item.Target)
            .Where(value => value is object)
            .ToArray();

        items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(T item)
    {
        cleanIfNeeded();

        for (int i = 0; i < inner.Count; i++)
        {
            var target = inner[i].Target as T;
            if (target == item)
            {
                inner.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public int Count
    {
        get
        {
            cleanIfNeeded();
            return inner.Count;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
    public bool IsReadOnly => false;
}
