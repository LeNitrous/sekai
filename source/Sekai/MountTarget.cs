// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

/// <summary>
/// Enumeration of mounting points of a <see cref="Framework.Storages.MountableStorage"/>.
/// </summary>
public enum MountTarget
{
    /// <summary>
    /// The game directory.
    /// </summary>
    Game,

    /// <summary>
    /// The user data directory.
    /// </summary>
    Data,

    /// <summary>
    /// The temporary directory.
    /// </summary>
    Temp
}
