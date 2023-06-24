// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.ComponentModel;
using Sekai.Audio;

namespace Sekai.Hosting;

/// <summary>
/// An <see cref="AudioDevice"/> provider.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IAudioProvider
{
    /// <summary>
    /// Creates an <see cref="AudioDevice"/>.
    /// </summary>
    AudioDevice CreateAudio();
}
