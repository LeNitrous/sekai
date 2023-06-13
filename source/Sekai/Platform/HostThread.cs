// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// An enumeration of host threads.
/// </summary>
public enum HostThread
{
    /// <summary>
    /// The main thread. This is usually where <see cref="Host.Run(Sekai.Game)"/> is called.
    /// </summary>
    Main,

    /// <summary>
    /// The game thread. This is created in the background.
    /// </summary>
    Game,
}
