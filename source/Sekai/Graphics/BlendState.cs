// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// A blending state.
/// </summary>
public abstract class BlendState : IDisposable
{
    /// <summary>
    /// Whether blending is enabled or not.
    /// </summary>
    public abstract bool Enabled { get; }

    /// <summary>
    /// The source color blending.
    /// </summary>
    public abstract BlendType SourceColor { get; }

    /// <summary>
    /// The destination color blending.
    /// </summary>
    public abstract BlendType DestinationColor { get; }

    /// <summary>
    /// The operation to perform between the <see cref="SourceColor"/> and <see cref="DesinationColor"/>.
    /// </summary>
    public abstract BlendOperation ColorOperation { get; }

    /// <summary>
    /// The source alpha blending.
    /// </summary>
    public abstract BlendType SourceAlpha { get; }

    /// <summary>
    /// The destination alpha blending.
    /// </summary>
    public abstract BlendType DestinationAlpha { get; }

    /// <summary>
    /// The operation to perform between the <see cref="SourceAlpha"/> and <see cref="DesinationAlpha"/>.
    /// </summary>
    public abstract BlendOperation AlphaOperation { get; }

    /// <summary>
    /// The color mask.
    /// </summary>
    public abstract ColorWriteMask WriteMask { get; }

    public abstract void Dispose();
}
