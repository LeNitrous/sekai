// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;

namespace Sekai.Hosting;

/// <summary>
/// An attribute used to denote a given type provides audio.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class AudioProviderAttribute : Attribute, IProviderAttribute
{
    public Type Type { get; }

    public AudioProviderAttribute(Type type)
    {
        Type = type;
    }
}
