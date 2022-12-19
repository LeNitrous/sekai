// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Allocation;

/// <summary>
/// The services singleton containing cached services for the game's lifetime.
/// </summary>
public static class Services
{
    /// <summary>
    /// The service container singleton.
    /// </summary>
    public static ServiceContainer Current => current is not null
        ? current
        : throw new InvalidOperationException(@"Service container is not yet initialized.");

    /// <summary>
    /// Creates a scoped service container.
    /// </summary>
    /// <remarks>
    /// The scoped service container inherits cached services from the global instance
    /// but it can also cache its own services without polluting the global instance.
    /// </remarks>
    public static ServiceContainer CreateScoped() => current is not null
        ? current.CreateScoped()
        : throw new InvalidOperationException(@"Service container is not yet initialized.");

    private static ServiceContainer.Global? current;

    internal static void Initialize()
    {
        current?.Dispose();
        current = new();
    }
}
