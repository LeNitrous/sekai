// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace Sekai.Windows;

public static class WindowsGameBuilderExtensions
{
    private static bool hasInitialized;

    /// <summary>
    /// Attempts to use the Windows App SDK for the given game.
    /// </summary>
    public static GameBuilder<T> UseWindowsSDK<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .ConfigureServices(services =>
            {
                services.Cache<ReunionWindow>();
            })
            .AddBuildAction(() =>
            {
                try
                {
                    hasInitialized = Bootstrap.TryInitialize(0x00010000, out int hresult);
                }
                catch
                {
                }
            })
            .AddExitAction(() =>
            {
                if (hasInitialized)
                    Bootstrap.Shutdown();
            });
    }
}
