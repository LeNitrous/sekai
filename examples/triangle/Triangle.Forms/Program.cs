// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Forms;
using Sekai.OpenGL;
using Triangle;

namespace Triangle.Forms;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Sekai.Game.Setup<TriangleGame>()
            .UseForms()
            .UseGL()
            .Run();
    }
}
