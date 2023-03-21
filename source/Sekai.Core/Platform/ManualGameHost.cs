// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform;

/// <summary>
/// A game host whose lifetime events are manually invoked.
/// </summary>
public sealed class ManualGameHost : IGameHost
{
    public IServiceLocator Services => services;

    public event Action? Create;
    public event Action? Load;
    public event Action? Tick;
    public event Action? Paused;
    public event Action? Resumed;
    public event Action? Unload;
    public event Action? Destroy;

    private readonly ServiceLocator services = new();

    public ManualGameHost()
    {
        services.Add(DefaultWaitableObjectFactory.Instance);
    }

    /// <summary>
    /// Calls the <see cref="Create"/> event.
    /// </summary>
    public void DoCreate()
    {
        Create?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Load"/> event.
    /// </summary>
    public void DoLoad()
    {
        Load?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Tick"/> event.
    /// </summary>
    public void DoTick()
    {
        Tick?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Pause"/> event.
    /// </summary>
    public void DoPause()
    {
        Paused?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Resume"/> event.
    /// </summary>
    public void DoResume()
    {
        Resumed?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Unload"/> event.
    /// </summary>
    public void DoUnload()
    {
        Unload?.Invoke();
    }

    /// <summary>
    /// Calls the <see cref="Destroy"/> event.
    /// </summary>
    public void DoDestroy()
    {
        Destroy?.Invoke();
    }
}
