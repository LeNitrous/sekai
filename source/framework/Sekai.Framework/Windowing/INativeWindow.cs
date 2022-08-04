// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Windowing;

public interface INativeWindow
{
    /// <summary>
    /// The native handle for this window.
    /// </summary>
    nint Handle { get; }
}
