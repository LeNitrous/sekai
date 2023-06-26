// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// Represents the host's platform.
/// </summary>
public abstract class Platform : IDisposable
{
    /// <summary>
    /// Gets the primary monitor.
    /// </summary>
    public abstract IMonitor PrimaryMonitor { get; }

    /// <summary>
    /// Gets all of the available monitors.
    /// </summary>
    public abstract IEnumerable<IMonitor> Monitors { get; }

    /// <summary>
    /// The options passed during host initialization.
    /// </summary>
    protected HostOptions Options { get; }

    protected Platform(HostOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Performs platform events.
    /// </summary>
    public virtual void DoEvents()
    {
    }

    /// <summary>
    /// Creates a window.
    /// </summary>
    public abstract IWindow CreateWindow();

    /// <summary>
    /// Creates storage.
    /// </summary>
    /// <remarks>
    /// Platform storage must implement the following mounts:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Path</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>/game/</term>
    ///         <description>A read-only storage containing game assets.</description>
    ///     </item>
    ///     <item>
    ///         <term>/user/</term>
    ///         <description>A read-write storage containing user data.</description>
    ///     </item>
    ///     <item>
    ///         <term>/cache/</term>
    ///         <description>A read-write storage for temporary storage.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <returns></returns>
    public abstract Storage CreateStorage();

    public abstract void Dispose();
}
