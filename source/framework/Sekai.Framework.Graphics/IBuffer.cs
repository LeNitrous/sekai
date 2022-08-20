// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// A device resource used to store arbitrary data in various formats.
/// </summary>
public interface IBuffer : IBindableResource
{
    /// <summary>
    /// The buffer's capacity in bytes.
    /// </summary>
    uint Size { get; }

    /// <summary>
    /// Determines how this buffer is used.
    /// </summary>
    BufferUsage Usage { get; }
}
