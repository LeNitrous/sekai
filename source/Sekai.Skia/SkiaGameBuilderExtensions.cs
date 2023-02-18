// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;

namespace Sekai.Skia;

public static class SkiaGameBuilderExtensions
{
    /// <summary>
    /// Adds Skia rendering support to the game.
    /// </summary>
    /// <returns>The service collection.</returns>
    public static IServiceCollection UseSkia(this IServiceCollection services)
    {
        return services.AddSingleton<SkiaContext>();
    }
}
