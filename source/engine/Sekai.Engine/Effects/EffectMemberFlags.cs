// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Engine.Effects;

[Flags]
public enum EffectMemberFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// A vertex attribute.
    /// </summary>
    Staged = 1 << 0,

    /// <summary>
    /// A buffer.
    /// </summary>
    Buffer = 1 << 1,

    /// <summary>
    /// A uniform.
    /// </summary>
    Uniform = 1 << 2,

    /// <summary>
    /// A texture.
    /// </summary>
    Texture = 1 << 3,

    /// <summary>
    /// A sampler.
    /// </summary>
    Sampler = 1 << 4,

    /// <summary>
    /// Determines that this member is a cubemap. Can only be used by <see cref="Texture"/> members.
    /// </summary>
    Cubemap = 1 << 5,

    /// <summary>
    /// Determines that this member can be read and be written to. Can only be used by <see cref="Texture"/> and <see cref="Buffer"/> members.
    /// </summary>
    ReadWrite = 1 << 6,
}
