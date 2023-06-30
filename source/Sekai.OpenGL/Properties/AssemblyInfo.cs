// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Runtime.CompilerServices;
using Sekai.Hosting;
using Sekai.OpenGL;

[assembly: GraphicsProvider(typeof(GLGraphicsProvider))]
[assembly: InternalsVisibleTo("Sekai.OpenGL.Tests")]
