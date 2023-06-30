// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// A rasterizer state.
/// </summary>
public abstract class RasterizerState : IDisposable
{
    /// <summary>
    /// The face to cull.
    /// </summary>
    public abstract FaceCulling Culling { get; }

    /// <summary>
    /// The winding order of the vertices.
    /// </summary>
    public abstract FaceWinding Winding { get; }

    /// <summary>
    /// The fill mode of primitives.
    /// </summary>
    public abstract FillMode Mode { get; }

    /// <summary>
    /// Whether to enable or disable scissor testing.
    /// </summary>
    public abstract bool Scissor { get; }

    public abstract void Dispose();
}
