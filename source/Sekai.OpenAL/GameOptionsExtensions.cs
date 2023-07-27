// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.OpenAL;

public static class GameOptionsExtensions
{
    /// <summary>
    /// Use OpenAL as the audio provider.
    /// </summary>
    public static void UseOpenAL(this GameOptions options)
    {
        options.Audio.CreateDevice = static () => new ALAudioDevice();
    }
}
