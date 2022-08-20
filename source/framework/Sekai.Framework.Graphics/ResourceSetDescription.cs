// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct ResourceSetDescription : IEquatable<ResourceSetDescription>
{
    /// <summary>
    /// Determines the layout of this resource set.
    /// </summary>
    public IResourceLayout Layout;

    /// <summary>
    /// The resources to be used in the pipeline.
    /// </summary>
    public IReadOnlyList<IBindableResource> Resources;

    public ResourceSetDescription(IResourceLayout layout, IReadOnlyList<IBindableResource> resources)
    {
        Layout = layout;
        Resources = resources;
    }

    public override bool Equals(object? obj)
    {
        return obj is ResourceSetDescription description && Equals(description);
    }

    public bool Equals(ResourceSetDescription other)
    {
        return EqualityComparer<IResourceLayout>.Default.Equals(Layout, other.Layout) &&
               EqualityComparer<IReadOnlyList<IBindableResource>>.Default.Equals(Resources, other.Resources);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Layout, Resources);
    }

    public static bool operator ==(ResourceSetDescription left, ResourceSetDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ResourceSetDescription left, ResourceSetDescription right)
    {
        return !(left == right);
    }
}
