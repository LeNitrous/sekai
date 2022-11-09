// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Shaders;

namespace Sekai.OpenGL;

internal class GLUniform<T> : INativeUniform, IUniform<T>
    where T : unmanaged, IEquatable<T>
{
    public GLShader Owner { get; }
    public string Name { get; }
    public int Offset { get; }

    public T Value
    {
        get => value;
        set
        {
            if (this.value.Equals(value))
                return;

            this.value = value;
            Owner.UpdateUniform(this);
        }
    }

    private T value;

    public GLUniform(GLShader owner, string name, int offset)
    {
        Name = name;
        Owner = owner;
        Offset = offset;
        value = default;
    }

    public ref T GetValueByRef() => ref value;

    INativeShader INativeUniform.Owner => Owner;
}
