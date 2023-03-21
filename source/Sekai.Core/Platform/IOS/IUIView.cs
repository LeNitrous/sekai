// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.IOS;

/// <summary>
/// An interface for iOS UI views.
/// </summary>
public interface IUIView
{
    /// <summary>
    /// The UI view handle.
    /// </summary>
    nint Handle { get; }
}
