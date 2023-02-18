// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Graphics.Shaders;

internal abstract class GlobalUniform : DisposableObject, IUniform
{
    protected IReadOnlySet<IUniform> Uniforms => uniforms;
    private readonly HashSet<IUniform> uniforms = new();

    public string Name { get; }

    public GlobalUniform(string name)
    {
        Name = name;
    }

    public abstract void Reset();
    public virtual void Attach(IUniform uniform) => uniforms.Add(uniform);
    public virtual void Detach(IUniform uniform) => uniforms.Remove(uniform);
    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
            uniforms.Clear();
    }
}

internal class GlobalUniform<T> : GlobalUniform, IUniform<T>
    where T : unmanaged, IEquatable<T>
{
    public T Value
    {
        get => value;
        set
        {
            if (this.value.Equals(value))
                return;

            this.value = value;

            foreach (var uniform in Uniforms)
            {
                if (uniform is IUniform<T> uniformValue)
                    uniformValue.Value = this.value;
            }
        }
    }

    public T Default { get; }

    private T value;

    public GlobalUniform(string name, T defaultValue = default)
        : base(name)
    {
        Default = value = defaultValue;
    }

    public override void Reset() => Value = Default;

    public override void Attach(IUniform uniform)
    {
        if (uniform is not IUniform<T> uniformValue)
            throw new InvalidOperationException();

        uniformValue.Value = Value;

        base.Attach(uniform);
    }
}
