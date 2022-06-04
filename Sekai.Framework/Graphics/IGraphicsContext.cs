// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.Windowing;
using Veldrid;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    IView View { get; }
    CommandList? Commands { get; }
    GraphicsDevice? Device { get; }
}
