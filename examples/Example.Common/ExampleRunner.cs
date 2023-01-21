// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;
using Sekai.OpenAL;
using Sekai.OpenGL;
using Sekai.SDL;

#if WINDOWS7_0_OR_GREATER
using Sekai.Windows;
#endif

namespace Example.Common;

public static class ExampleRunner
{
    public static void Run<T>()
        where T : Game, new()
    {
        Game.Setup<T>()
            .UseSDL()
            .UseGL()
            .UseAL()
#if WINDOWS7_0_OR_GREATER
            .UseWindowsSDK()
#endif
            .Run();
    }
}
