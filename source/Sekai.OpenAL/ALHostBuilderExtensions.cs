// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.OpenAL;

public static class ALHostBuilderExtensions
{
    /// <summary>
    /// Uses the OpenAL Audio System.
    /// </summary>
    public static IHostBuilder UseAL(this IHostBuilder builder)
    {
        return builder.UseAudio<ALAudioSystem>();
    }
}
