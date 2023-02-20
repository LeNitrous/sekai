// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using Sekai.Allocation;
using Sekai.Logging;
using Sekai.Storages;

namespace Sekai;

public sealed class GameBuilder
{
    /// <summary>
    /// A collection of services that can be composed by the user used to build the <see cref="Game"/>.
    /// </summary>
    public IServiceCollection Services => services;

    /// <summary>
    /// The host builder used in building the <see cref="IHost"/>.
    /// </summary>
    public IHostBuilder Host => host;

    private readonly HostBuilder host;
    private readonly ServiceCollection services = new();

    internal GameBuilder(Func<Game> creator, GameOptions options)
    {
        host = new(creator, options, services);
    }

    /// <summary>
    /// Builds the host.
    /// </summary>
    public IHostStartup Build()
    {
        return host.Build();
    }
}
