// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    GraphicsAPI API { get; }
}
