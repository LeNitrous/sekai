// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public enum ResourceKind
{
    /// <summary>
    /// A <see cref="IBuffer"/> accessed as a uniform buffer.
    /// </summary>
    UniformBuffer,

    /// <summary>
    /// A <see cref="IBuffer"/> accessed as a read-only storage buffer.
    /// </summary>
    StructuredBufferReadOnly,

    /// <summary>
    /// A <see cref="IBuffer"/> accessed as a read-write storage buffer.
    /// </summary>
    StructuredBufferReadWrite,

    /// <summary>
    /// A read-only <see cref="INativeTexture"/>.
    /// /// </summary>
    TextureReadOnly,

    /// <summary>
    /// A read-write <see cref="INativeTexture"/>.
    /// </summary>
    TextureReadWrite,

    /// <summary>
    /// A <see cref="ISampler"/>.
    /// </summary>
    Sampler,
}
