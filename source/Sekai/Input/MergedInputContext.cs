// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Input;

internal sealed class MergedInputSource : IInputSource, ICollection<IInputSource>
{
    public event Action<IInputDevice, bool>? ConnectionChanged;
    public IEnumerable<IInputDevice> Devices => contexts.SelectMany(c => c.Devices);

    int ICollection<IInputSource>.Count => throw new NotImplementedException();

    bool ICollection<IInputSource>.IsReadOnly => throw new NotImplementedException();

    private readonly HashSet<IInputSource> contexts = new();

    public void Add(IInputSource context)
    {
        if (!contexts.Add(context))
        {
            return;
        }

        context.ConnectionChanged += handleConnectionChanged;
    }

    public bool Remove(IInputSource context)
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

    public bool Contains(IInputSource item)
    {
        return contexts.Contains(item);
    }

    public IEnumerator<IInputSource> GetEnumerator()
    {
        return contexts.GetEnumerator();
    }

    private void handleConnectionChanged(IInputDevice device, bool connected)
    {
        ConnectionChanged?.Invoke(device, connected);
    }

    void ICollection<IInputSource>.CopyTo(IInputSource[] array, int arrayIndex)
    {
        contexts.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
