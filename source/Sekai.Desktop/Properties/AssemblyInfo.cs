// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Runtime.CompilerServices;
using Sekai.Desktop;
using Sekai.Hosting;

[assembly: HostComponent(typeof(DesktopProvider))]
[assembly: InternalsVisibleTo("Sekai.OpenGL.Tests")]
