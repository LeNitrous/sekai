// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework.Entities;

public abstract class Drawable : Component, IRenderable
{
    public abstract void Render(CommandList commands);
}
