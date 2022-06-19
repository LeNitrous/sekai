// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    /// <summary>
    /// The current graphics API this context is using.
    /// </summary>
    GraphicsAPI API { get; }

    /// <summary>
    /// Whether to enable or disable vertical syncing.
    /// </summary>
    bool VSync { get; set; }
}
