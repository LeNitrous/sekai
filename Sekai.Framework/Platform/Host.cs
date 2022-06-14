// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Entities;
using Sekai.Framework.IO.Storage;
using Sekai.Framework.Logging;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public abstract class Host : FrameworkObject
{
    private GameThreadManager? threads;
    private GameSystemRegistry? systems;
    private GameThread? mainThread;

    public void Run(Game game)
    {
        Logger.OnMessageLogged += new LogListenerConsole();

        threads = CreateThreadManager();
        threads.ExecutionMode = ExecutionMode.MultiThread;
        threads.FramesPerSecond = 120;
        threads.UpdatePerSecond = 240;
        threads.Add(new UpdateThread(update));
        mainThread = CreateMainThread();
        mainThread.PropagatesExceptions = true;

        systems = new(game);
        systems.Register<SceneManager>();

        game.Services.Cache(this);
        game.Services.Cache(threads);
        game.Services.Cache(systems);
        game.Services.Cache(CreateStorage());

        Initialize(game);

        mainThread.Dispatch(game.Initialize);
        threads.Run(mainThread);

        void update(double delta)
        {
            if (systems != null)
            {
                foreach (var system in systems.OfType<IUpdateable>())
                    system.Update(delta);
            }

            game.Update(delta);
        }
    }

    protected abstract GameThread CreateMainThread();

    protected virtual GameThreadManager CreateThreadManager()
    {
        return new();
    }

    protected virtual IStorage CreateStorage()
    {
        return new VirtualStorage();
    }

    protected virtual void Initialize(Game game)
    {
    }

    protected override void Destroy()
    {
        systems?.Dispose();
        threads?.Dispose();
    }

    public static Host GetSuitableHost()
    {
        if (RuntimeInfo.IsDesktop)
            return new DesktopHost();

        if (RuntimeInfo.IsMobile)
            return new MobileHost();

        throw new PlatformNotSupportedException(@"Failed to find suitable host for the current platform.");
    }
}
