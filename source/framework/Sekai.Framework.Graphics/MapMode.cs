// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public enum MapMode
{
    /// <summary>
    /// A read-only resource mapping. The mapped data region is not writable, and cannot be used to transfer data into the
    /// graphics resource. NOTE: This mode can only be used on resources created with the Staging usage flag.
    /// </summary>
    Read,

    /// <summary>
    /// A write-only resource mapping. The mapped data region is writable, and will be transferred into the graphics resource
    /// when <see cref="IGraphicsDevice.Unmap"/> is called. NOTE: upon mapping a buffer with this
    /// mode, the previous contents of the resource will be erased. This mode can only be used to entirely replace the
    /// contents of a resource.
    /// </summary>
    Write,

    /// <summary>
    /// A read-write resource mapping. The mapped data region is both readable and writable. NOTE: this mode can only be used
    /// on resources created with the Staging usage flag.
    /// </summary>
    ReadWrite,
}
