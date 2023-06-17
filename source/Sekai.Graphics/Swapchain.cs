// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Graphics;

/// <summary>
/// Contains a collection of buffers used for presentation.
/// </summary>
public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// The swapchain's size.
    /// </summary>
    public abstract Size Size { get; set; }

    public abstract void Dispose();
}
