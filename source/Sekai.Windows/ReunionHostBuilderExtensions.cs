// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.ComponentModel;
using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace Sekai.Windows;

public static class ReunionHostBuilderExtensions
{
    private static ReunionWindow? window;

    /// <summary>
    /// Attempts to use the Windows App SDK for the given host.
    /// </summary>
    public static IHostBuilder UseWindowsSDK(this IHostBuilder builder)
    {
        builder
            .UseInit(init)
            .UseExit(exit);

        return builder;
    }
    
    private static void init(IHost host)
    {
        if (Bootstrap.TryInitialize(0x00010002, out int hresult))
        {
            window = new ReunionWindow(host.Surface);
            host.Logger.Log("Windows Apps SDK initialized.");
        }
        else
        {
            host.Logger.Error("Failed to start Windows Apps SDK.", new Win32Exception(hresult));
        }
    }

    private static void exit(IHost host)
    {
        if (window is not null)
        {
            window.Dispose();
            Bootstrap.Shutdown();
        }
    }
}
