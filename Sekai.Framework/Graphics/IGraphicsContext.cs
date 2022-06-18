// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Veldrid;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    GraphicsAPI API { get; }
    GraphicsDevice Device { get; }
    ResourceFactory Resources { get; }
}
