// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics.Vertices;

/// <summary>
/// Describes a vertex layout.
/// </summary>
public interface IVertexLayout : IEquatable<IVertexLayout>
{
    /// <summary>
    /// The size of this vertex layout in bytes.
    /// </summary>
    int Stride { get; }

    /// <summary>
    /// The members defined in this layout.
    /// </summary>
    IReadOnlyList<VertexMember> Members { get; }
}
