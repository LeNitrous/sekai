// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// The mode of how a resource will be mapped.
/// </summary>
public enum MapMode
{
    /// <summary>
    /// The resource is mapped as read-only.
    /// </summary>
    Read,

    /// <summary>
    /// The resource is mapped as write-only.
    /// </summary>
    Write,

    /// <summary>
    /// The resource is mapped as read/write.
    /// </summary>
    ReadWrite,
}
