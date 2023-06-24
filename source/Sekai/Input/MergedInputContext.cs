// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Input;

/// <summary>
/// An input context that merges other input contexts.
/// </summary>
public sealed class MergedInputContext : IInputContext, ICollection<IInputContext>
{
    public event Action<IInputDevice, bool>? ConnectionChanged;
    public IEnumerable<IInputDevice> Devices => contexts.SelectMany(c => c.Devices);

    int ICollection<IInputContext>.Count => throw new NotImplementedException();

    bool ICollection<IInputContext>.IsReadOnly => throw new NotImplementedException();

    private readonly HashSet<IInputContext> contexts = new();

    public void Add(IInputContext context)
    {
        if (!contexts.Add(context))
        {
            return;
        }

        context.ConnectionChanged += handleConnectionChanged;
    }

    public bool Remove(IInputContext context)
    {
        if (!contexts.Remove(context))
        {
            return false;
        }

        context.ConnectionChanged -= handleConnectionChanged;

        return true;
    }

    public void Clear()
    {
        foreach (var context in contexts)
        {
            context.ConnectionChanged -= handleConnectionChanged;
        }

        contexts.Clear();
    }

    public bool Contains(IInputContext item)
    {
        return contexts.Contains(item);
    }

    public IEnumerator<IInputContext> GetEnumerator()
    {
        return contexts.GetEnumerator();
    }

    private void handleConnectionChanged(IInputDevice device, bool connected)
    {
        ConnectionChanged?.Invoke(device, connected);
    }

    void ICollection<IInputContext>.CopyTo(IInputContext[] array, int arrayIndex)
    {
        contexts.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
