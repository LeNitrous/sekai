// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Reflection;
using Sekai;
using Sekai.OpenAL;
using Sekai.OpenGL;
using Sekai.SDL;
using Sekai.Storages;

#if WINDOWS7_0_OR_GREATER
using Sekai.Windows;
#endif

namespace Example.Common;

public static class ExampleRunner
{
    public static void Run<T>()
        where T : Game, new()
    {
        var builder = Game.Setup<T>(new GameOptions { Name = "Sekai.Examples"});

        builder.Host
            .UseSDL()
            .UseGL()
            .UseAL()
#if WINDOWS7_0_OR_GREATER
            .UseWindowsSDK();
#else
            ;
#endif

        builder.Host.UseStorage("./game", new AssemblyStorage(Assembly.GetEntryAssembly()!).GetStorage("./Resources"));

        builder.Build().Run();
    }
}
