// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Hosting;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class HostComponentAttribute : Attribute
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type Type { get; }

    public HostComponentAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
    {
        Type = type;
    }
}
