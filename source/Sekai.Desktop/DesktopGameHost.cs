// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Platform;

namespace Sekai.Desktop;

/// <summary>
/// A game host that has a window.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
public class DesktopGameHost : Host
{
    public override IView? View => window;

    private readonly IWindow window;

    /// <summary>
    /// Creates a new instance of a desktop game host.
    /// </summary>
    public DesktopGameHost()
    {
        window = CreateWindow();
        window.State = WindowState.Hidden;
        window.Border = WindowBorder.Resizable;
    }

    protected override void Run()
    {
        window.Resume += Resume;
        window.Suspend += Pause;
        window.Closing += Exit;

        Task.Factory.StartNew(doGameLoop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        while (State != HostState.Exited)
        {
            window.DoEvents();
        }

        window.Resume -= Resume;
        window.Suspend -= Pause;
        window.Closing -= Exit;

        window.Dispose();
    }

    private void doGameLoop()
    {
        bool hasFirstTick = false;

        while (State != HostState.Exited)
        {
            DoTick();

            if (!hasFirstTick)
            {
                window.State = WindowState.Normal;
                hasFirstTick = true;
            }
        }
    }

    /// <summary>
    /// Creates a window for the game host.
    /// </summary>
    /// <returns>A window.</returns>
    protected virtual IWindow CreateWindow() => new Window();
}
