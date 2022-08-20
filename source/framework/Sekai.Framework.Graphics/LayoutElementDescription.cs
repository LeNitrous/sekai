// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct LayoutElementDescription : IEquatable<LayoutElementDescription>
{
    /// <summary>
    /// The name of the element.
    /// </summary>
    public string Name;

    /// <summary>
    /// The kind of resource this element is.
    /// </summary>
    public ResourceKind Kind;

    /// <summary>
    /// The shader stages where this element is used in.
    /// </summary>
    public ShaderStage Stages;

    /// <summary>
    /// Miscellaneous options for this element.
    /// </summary>
    public LayoutElementFlags Flags;

    public LayoutElementDescription(string name, ResourceKind kind, ShaderStage stages, LayoutElementFlags flags)
    {
        Name = name;
        Kind = kind;
        Stages = stages;
        Flags = flags;
    }

    public override bool Equals(object? obj)
    {
        return obj is LayoutElementDescription description && Equals(description);
    }

    public bool Equals(LayoutElementDescription other)
    {
        return Name == other.Name &&
               Kind == other.Kind &&
               Stages == other.Stages &&
               Flags == other.Flags;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Kind, Stages, Flags);
    }

    public static bool operator ==(LayoutElementDescription left, LayoutElementDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LayoutElementDescription left, LayoutElementDescription right)
    {
        return !(left == right);
    }
}
