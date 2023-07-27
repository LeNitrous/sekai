// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Runtime.Versioning;

namespace Sekai.GLFW;

public static class GameOptionsExtensions
{
    /// <summary>
    /// Use GLFW as the window and input provider.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("osx")]
    public static void UseGLFW(this GameOptions options)
    {
        options.View = new GameOptions.WindowOptions
        {
            Create = () => new GLFWWindow()
        };
    }
}
