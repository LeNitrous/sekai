// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Logging;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public abstract class Host : FrameworkObject
{
    private GameThreadManager? threads;
    private readonly GameSystemRegistry systems = new();

    public void Run(Game game)
    {
        Logger.OnMessageLogged += new LogListenerConsole();

        var mainThread = CreateMainThread();

        threads = new(mainThread)
        {
            ExecutionMode = ExecutionMode.MultiThread,
            FramesPerSecond = 120,
            UpdatePerSecond = 240,
        };

        threads.Add(new UpdateThread(update));

        game.Services.Cache(this);
        game.Services.Cache(threads);
        game.Services.Cache(systems);
        mainThread.Dispatch(game.Initialize);

        Initialize(game);

        threads.Run();

        void update(double delta)
        {
            foreach (var system in systems.OfType<IUpdateable>())
                system.Update(delta);

            game.Update(delta);
        }
    }

    protected abstract GameThread CreateMainThread();

    protected virtual void Initialize(Game game)
    {
    }

    protected override void Destroy()
    {
        systems.Dispose();
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
