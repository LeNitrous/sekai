using Sekai.OpenGL;
using Sekai.SDL;
using Triangle.Game;

Sekai.Game.Setup<TriangleGame>()
    .UseSDL()
    .UseGL()
    .Run();
