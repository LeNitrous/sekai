// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// Represents a global uniform whose value is shared with all shaders.
/// </summary>
public class GlobalUniform : IUniform
{
    /// <summary>
    /// The global uniform's name.
    /// </summary>
    public string Name { get; }

    internal IReadOnlySet<IUniform> Attached => attached;
    private readonly HashSet<IUniform> attached = new();

    public GlobalUniform(string name)
    {
        Name = name;
    }

    internal virtual void Attach(IUniform uniform)
    {
        attached.Add(uniform);
    }

    internal virtual void Detach(IUniform uniform)
    {
        attached.Remove(uniform);
    }
}

/// <summary>
/// Represents a global uniform whose value is shared with all shaders.
/// </summary>
public class GlobalUniform<T> : GlobalUniform, IUniform<T>
    where T : unmanaged, IEquatable<T>
{
    /// <summary>
    /// The global uniform's value.
    /// </summary>
    public T Value
    {
        get => value;
        set
        {
            if (value.Equals(this.value))
                return;

            this.value = value;

            foreach (var uniform in Attached)
                ((IUniform<T>)uniform).Value = value;
        }
    }

    private T value;

    public GlobalUniform(string name)
        : base(name)
    {
    }

    internal sealed override void Attach(IUniform uniform)
    {
        if (uniform is not IUniform<T> u)
            throw new InvalidOperationException();

        u.Value = value;
        base.Attach(uniform);
    }
}
