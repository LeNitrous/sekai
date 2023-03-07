// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sekai.Collections;

internal sealed class SortedFilteredCollection<T> : ICollection<T>
{
    public int Count => items.Count;

    private bool shouldRebuildCache;
    private readonly Predicate<T> filter;
    private readonly IComparer<T> sorter;
    private readonly List<T> items = new();
    private readonly List<T> cache = new();
    private readonly Action<T, EventHandler> filterChangedSubscriber;
    private readonly Action<T, EventHandler> filterChangedUnsubscriber;
    private readonly Action<T, EventHandler> sorterChangedSubscriber;
    private readonly Action<T, EventHandler> sorterChangedUnsubscriber;

    public SortedFilteredCollection(
        IComparer<T> sorter,
        Predicate<T> filter,
        Action<T, EventHandler> filterChangedSubscriber,
        Action<T, EventHandler> filterChangedUnsubscriber,
        Action<T, EventHandler> sorterChangedSubscriber,
        Action<T, EventHandler> sorterChangedUnsubscriber)
    {
        this.filter = filter;
        this.sorter = sorter;
        this.filterChangedSubscriber = filterChangedSubscriber;
        this.filterChangedUnsubscriber = filterChangedUnsubscriber;
        this.sorterChangedSubscriber = sorterChangedSubscriber;
        this.sorterChangedUnsubscriber = sorterChangedUnsubscriber;
    }

    public void Add(T item)
    {
        items.Add(item);
        invalidate();
    }

    public void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            unsubscribeFromEvents(items[i]);
        }

        items.Clear();
        invalidate();
    }

    public bool Contains(T item)
    {
        return items.Contains(item);
    }

    public bool Remove(T item)
    {
        if (!items.Remove(item))
        {
            return false;
        }

        unsubscribeFromEvents(item);
        invalidate();

        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (shouldRebuildCache)
        {
            cache.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (filter(item))
                {
                    cache.Add(item);
                    subscribeToEvents(item);
                }
            }

            if (cache.Count > 0)
            {
                cache.Sort(sorter);
            }

            shouldRebuildCache = false;
        }

        return cache.GetEnumerator();
    }

    private void invalidate()
    {
        shouldRebuildCache = true;
    }

    private void filterPropertyChanged(object? sender, EventArgs args)
    {
        invalidate();
    }

    private void sorterPropertyChanged(object? sender, EventArgs args)
    {
        unsubscribeFromEvents((T)sender!);
        invalidate();
    }

    private void subscribeToEvents(T item)
    {
        filterChangedSubscriber(item, filterPropertyChanged);
        sorterChangedSubscriber(item, sorterPropertyChanged);
    }

    private void unsubscribeFromEvents(T item)
    {
        filterChangedUnsubscriber(item, filterPropertyChanged);
        sorterChangedUnsubscriber(item, sorterPropertyChanged);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool ICollection<T>.IsReadOnly => false;

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);
}
