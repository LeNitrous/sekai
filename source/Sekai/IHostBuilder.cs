// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

public interface IHostBuilder
{
    /// <summary>
    /// The service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Uses an input system.
    /// </summary>
    /// <param name="system">The system to use.</param>
    /// <returns>The host builder.</returns>
    /// <remarks>
    /// This can be called multiple times to use multiple input systems.
    /// </remarks>
    IHostBuilder UseInput(InputSystem system);

    /// <inheritdoc cref="UseInput(InputSystem)"/>
    /// <typeparam name="T">The system to use.</typeparam>
    IHostBuilder UseInput<T>() where T : InputSystem, new();

    /// <summary>
    /// Uses an audio system.
    /// </summary>
    /// <param name="system">The system to use.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseAudio(AudioSystem system);

    /// <inheritdoc cref="UseAudio(AudioSystem)"/>
    /// <typeparam name="T">The system to use.</typeparam>
    IHostBuilder UseAudio<T>() where T : AudioSystem, new();

    /// <summary>
    /// Uses an asset loader.
    /// </summary>
    /// <typeparam name="T">The asset type to be loaded.</typeparam>
    /// <param name="loader">The asset loader to use.</param>
    /// <param name="extensions">A collection of extensions (with or without the period) that this loader can load.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseLoader<T>(IAssetLoader<T> loader, params string[] extensions) where T : notnull, IAsset;

    /// <inheritdoc cref="UseLoader{T}(IAssetLoader{T}, string[])"/>
    /// <typeparam name="U">The asset loader to use.</typeparam>
    IHostBuilder UseLoader<T, U>(params string[] extensions) where T : notnull, IAsset where U : class, IAssetLoader<T>, new();

    /// <summary>
    /// Uses a surface.
    /// </summary>
    /// <param name="surface">The surface to use.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseSurface(Surface surface);

    /// <inheritdoc cref="UseSurface(Surface)"/>
    /// <typeparam name="T">The surface to use.</typeparam>
    IHostBuilder UseSurface<T>() where T : Surface, new();

    /// <summary>
    /// Uses a graphics system.
    /// </summary>
    /// <param name="creator">The factory method to use in creating the graphics system.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseGraphics(Func<Surface, GraphicsSystem> creator);

    /// <summary>
    /// Uses a storage that is to be mounted at a specified mounting point.
    /// </summary>
    /// <param name="uri">The mount path of the storage.</param>
    /// <param name="storage">The storage to be mounted.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseStorage(Uri uri, Storage storage);

    /// <inheritdoc cref="UseStorage(Uri, Storage)"/>
    /// <param name="path">The mount path of the storage.</param>
    IHostBuilder UseStorage(string path, Storage storage);

    /// <summary>
    /// Uses actions to be used in a separate thread.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onInit">The action to perform when the thread starts.</param>
    /// <param name="onLoop">The action to perform every frame.</param>
    /// <param name="onExit">The action to perform when the thread is closing.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseThread(string name, Action? onInit = null, Action? onLoop = null, Action? onExit = null);

    /// <summary>
    /// Uses an action called before the game loads.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseInit(Action<IHost> action);

    /// <summary>
    /// Uses an action that is called after the game exits.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseExit(Action<IHost> action);

    /// <summary>
    /// Uses an action that is called during host building.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>The host builder.</returns>
    IHostBuilder UseAction(Action<IHostBuilder> action);
}
