// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;

namespace Sekai.Framework.Entities.Processors;

public class DrawableProcessor : EntityProcessor<Drawable>
{
    protected override void Render(Entity entity, Drawable component, CommandList commands)
    {
        component.Render(commands);
    }

    protected override void Update(Entity entity, Drawable component, double elapsed)
    {
    }
}
