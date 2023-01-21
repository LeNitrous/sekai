// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.OpenAL;

public static class ALGameBuilderExtensions
{
    /// <summary>
    /// Uses the OpenAL Audio System.
    /// </summary>
    public static GameBuilder<T> UseAL<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseAudio<ALAudioSystem>();
    }
}
