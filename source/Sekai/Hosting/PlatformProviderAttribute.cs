// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;

namespace Sekai.Hosting;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class PlatformProviderAttribute : Attribute, IProviderAttribute
{
    public Type Type { get; }

    public PlatformProviderAttribute(Type type)
    {
        Type = type;
    }
}
