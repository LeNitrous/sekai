using Example.Window;
using Sekai.Engine.Platform;
using Sekai.SDL;

Host
    .Setup<ExampleGame>()
    .UseWindow<SDLWindow>()
    .Run();
