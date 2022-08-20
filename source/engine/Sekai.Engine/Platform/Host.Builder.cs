// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Platform;

public partial class Host<T>
{
    private IWindow window = null!;
    private IGraphicsContext graphics = null!;
    private Action<T> callbackGameLoad = null!;

    /// <summary>
    /// Uses the window of a given type.
    /// </summary>
    public Host<T> UseWindow<U>()
        where U : IWindow, new()
    {
        window = new U();
        return this;
    }

    /// <summary>
    /// Uses the graphics context of a given type.
    /// </summary>
    public Host<T> UseGraphics<U>()
        where U : IGraphicsContext, new()
    {
        graphics = new U();
        return this;
    }

    /// <summary>
    /// Uses a callback that is invoked after the game has finished loading.
    /// </summary>
    public Host<T> UseLoadCallback(Action<T> callback)
    {
        callbackGameLoad = callback;
        return this;
    }

    private void setupHostInstances()
    {
        window.Size = options.Size;
        window.Title = options.Title;
        window.OnClose += Exit;

        options.Graphics.Debug ??= RuntimeInfo.IsDebug;
        options.Graphics.GraphicsAPI ??= getGraphicsAPIForPlatform();

        graphics.Initialize(window, options.Graphics);
    }

    private static GraphicsAPI getGraphicsAPIForPlatform()
    {
        return RuntimeInfo.OS switch
        {
            RuntimeInfo.Platform.Windows => GraphicsAPI.Direct3D11,
            RuntimeInfo.Platform.Android or RuntimeInfo.Platform.Linux => GraphicsAPI.Vulkan,
            RuntimeInfo.Platform.macOS or RuntimeInfo.Platform.iOS => GraphicsAPI.Metal,
            _ => GraphicsAPI.OpenGL,
        };
    }
}
