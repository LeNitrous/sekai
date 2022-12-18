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
    public static ServiceContainer Current => current;

    /// <summary>
    /// Creates a scoped service container.
    /// </summary>
    /// <remarks>
    /// The scoped service containerinherits cached services from the global instance
    /// but it can also cache its own services without polluting the global instance.
    /// </remarks>
    public static ServiceContainer CreateScoped() => current.CreateScoped();

    private static readonly ServiceContainer.Global current = new();
}

public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(string? message)
        : base(message)
    {
    }
}
