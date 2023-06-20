// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Input;
using Silk.NET.GLFW;

namespace Sekai.Desktop.Input;

internal interface IGLFWController : IController
{
    void Update(Glfw glfw);
}
