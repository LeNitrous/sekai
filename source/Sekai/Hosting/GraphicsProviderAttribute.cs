// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;

namespace Sekai.Hosting;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GraphicsProviderAttribute : Attribute, IProviderAttribute
{
    public Type Type { get; }

    public GraphicsProviderAttribute(Type type)
    {
        Type = type;
    }
}
