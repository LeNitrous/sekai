// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using Sekai.Engine.Resources;
using Sekai.Framework;
using Sekai.Framework.UI;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Storage;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Platform;

public partial class Host<T>
{
    private VirtualStorage storage = null!;
    private IWindow window = null!;
    private IUserInterface ui = null;
    private IGraphicsDevice graphics = null!;
    private Action<T> callbackGameLoad = null!;
    private Action<VirtualStorage> callbackStorageSetup = null!;

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
        where U : IGraphicsDevice, new()
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

    public Host<T> UseUserInterface<U>()
      where U: IUserInterface, new()
    {
        ui = new U();
        return this;
    }

    /// <summary>
    /// Uses a callback to setup storage.
    /// </summary>
    public Host<T> UseStorageCallback(Action<VirtualStorage> callback)
    {
        callbackStorageSetup = callback;
        return this;
    }

    private void setupHostInstances()
    {
        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        window.Size = options.Size;
        window.Title = options.Title;
        window.Border = WindowBorder.Resizable;
        window.OnClose += Exit;

        options.Graphics.Debug ??= RuntimeInfo.IsDebug;
        options.Graphics.GraphicsAPI ??= getGraphicsAPIForPlatform();

        graphics.Initialize(window, options.Graphics);
        Logger.Log($"{graphics.GraphicsAPI} Initialized");
        Logger.Log($"{graphics.GraphicsAPI} Device: {graphics.Name}");

        storage = new VirtualStorage();
        storage.Mount("/engine", new AssemblyBackedStorage(ResourceAssembly.Assembly));
        storage.Mount("/engine/logs", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")));
        storage.Mount("/engine/cache", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache")));
        callbackStorageSetup?.Invoke(storage);
    }

    private static GraphicsAPI getGraphicsAPIForPlatform()
    {
        return RuntimeInfo.OS switch
        {
            RuntimeInfo.Platform.Windows => GraphicsAPI.Direct3D11,
            RuntimeInfo.Platform.Android or RuntimeInfo.Platform.Linux => GraphicsAPI.Vulkan,
            RuntimeInfo.Platform.macOS or RuntimeInfo.Platform.iOS => GraphicsAPI.Metal,
            _ => RuntimeInfo.IsMobile ? GraphicsAPI.OpenGLES : GraphicsAPI.OpenGL,
        };
    }
}
