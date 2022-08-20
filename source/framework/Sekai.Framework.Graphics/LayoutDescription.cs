// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct LayoutDescription : IEquatable<LayoutDescription>
{
    /// <summary>
    /// A list of elements describing the layout of this resource.
    /// </summary>
    public IReadOnlyList<LayoutElementDescription> Elements;

    public LayoutDescription(IReadOnlyList<LayoutElementDescription> elements)
    {
        Elements = elements;
    }

    public override bool Equals(object? obj)
    {
        return obj is LayoutDescription description && Equals(description);
    }

    public bool Equals(LayoutDescription other)
    {
        return EqualityComparer<IReadOnlyList<LayoutElementDescription>>.Default.Equals(Elements, other.Elements);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Elements);
    }

    public static bool operator ==(LayoutDescription left, LayoutDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LayoutDescription left, LayoutDescription right)
    {
        return !(left == right);
    }
}
