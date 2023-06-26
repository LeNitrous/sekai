// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.ComponentModel;

namespace Sekai.Hosting;

/// <summary>
/// A <see cref="Platform"/> provider.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IPlatformProvider
{
    /// <summary>
    /// Creates a <see cref="Platform"/>.
    /// </summary>
    /// <param name="options">The options passed during initialization.</param>
    Platform CreatePlatform(HostOptions options);
}
