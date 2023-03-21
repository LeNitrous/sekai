// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.Android;

/// <summary>
/// An interface used for android surfaces.
/// </summary>
public interface IAndroidSurface
{
    /// <summary>
    /// The android's surface.
    /// </summary>
    nint Surface { get; }

    /// <summary>
    /// The android's Java Native Interop handle.
    /// </summary>
    nint JNIHandle { get; }
}
