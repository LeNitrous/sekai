// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Hosting;

/// <summary>
/// An attribute used to denote a given type provides audio.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class AudioProviderAttribute : Attribute, IProviderAttribute
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type Type { get; }

    public AudioProviderAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
    {
        Type = type;
    }
}
