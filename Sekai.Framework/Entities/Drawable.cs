// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Entities.Processors;
using Sekai.Framework.Graphics;

namespace Sekai.Framework.Entities;

/// <summary>
/// A <see cref="Component"/> that renders every frame through <see cref="Render"/>.
/// </summary>
[EntityProcessor(typeof(DrawableProcessor))]
public abstract class Drawable : Component, IRenderable
{
    public abstract void Render(CommandList commands);
}
