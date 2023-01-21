// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Shaders;

namespace Sekai.OpenGL;

internal abstract class GLUniform : IUniform
{
    public string Name { get; }
    public int Offset { get; }
    public uint Owner { get; }

    public GLUniform(uint owner, string name, int offset)
    {
        Name = name;
        Owner = owner;
        Offset = offset;
    }
}

internal class GLUniform<T> : GLUniform, IUniform<T>
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
            system.UpdateShaderUniform(this);
        }
    }

    private T value;
    private readonly GLGraphicsSystem system;

    public GLUniform(GLGraphicsSystem system, uint owner, string name, int offset)
        : base(owner, name, offset)
    {
        this.system = system;
    }

    public ref T GetValueByRef() => ref value;
}
