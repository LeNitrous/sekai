// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
public sealed class CachedAttribute : Attribute
{
    public Type? AsType { get; set; }

    public CachedAttribute(Type? asType = null)
    {
        AsType = asType;
    }
}
