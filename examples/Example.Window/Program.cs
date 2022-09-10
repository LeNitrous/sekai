// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Example.Window;
using Sekai.Engine.Platform;
using Sekai.ImGui;
using Sekai.SDL;
using Sekai.Veldrid;

Host
    .Setup<ExampleGame>()
    .UseSDLWindow()
    .UseVeldrid()
    .UseImGui()
    .Run();
