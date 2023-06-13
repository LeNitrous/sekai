// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;

namespace Sekai.Platform;

/// <summary>
/// Exposes factory methods provided by an <see cref="IHost"/>.
/// </summary>
internal interface IHostFactory
{
    /// <summary>
    /// Creates an input source.
    /// </summary>
    InputSource CreateInput();

    /// <summary>
    /// Creates an audio device.
    /// </summary>
    AudioDevice CreateAudio();

    /// <summary>
    /// Creates a graphics device.
    /// </summary>
    GraphicsDevice CreateGraphics();
}
