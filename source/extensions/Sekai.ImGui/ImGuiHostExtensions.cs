// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;
using Sekai.Framework.Storage;

namespace Sekai.ImGui;

public static class ImGuiHostExtensions
{
    public static HostBuilder<T> UseImGui<T>(this HostBuilder<T> builder)
        where T : Game, new()
    {
        builder.Mount("/engine/imgui", new AssemblyBackedStorage(typeof(ImGuiHostExtensions).Assembly));
        builder.Register<ImGuiController>();
        return builder;
    }
}
