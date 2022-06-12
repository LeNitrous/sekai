// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics.CodeAnalysis;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

namespace Sekai.Framework.Graphics;

public abstract class GraphicsContext : FrameworkObject, IGraphicsContext
{
    [AllowNull]
    public GraphicsDevice Device { get; private set; }
    protected abstract GraphicsBackend Backend { get; }

    public void Initialize(IView view)
    {
        Device = view.CreateGraphicsDevice
        (
            new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            }, Backend
        );

        Initialize();
    }

    protected virtual void Initialize()
    {
    }

    protected sealed override void Destroy()
    {
        Device.Dispose();
    }
}
