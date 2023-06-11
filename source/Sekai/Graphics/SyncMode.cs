// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// Enumeration of sync modes.
/// </summary>
public enum SyncMode
{
    /// <summary>
    /// Disable vertical syncing.
    /// </summary>
    None,

    /// <summary>
    /// Always sync to vertical blank.
    /// </summary>
    Traditional,

    /// <summary>
    /// Only sync to vertical blank as needed.
    /// </summary>
    Adaptive,
}
