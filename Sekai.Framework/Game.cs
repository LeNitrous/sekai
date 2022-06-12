// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework;

public abstract class Game : LoadableObject, IUpdateable, IRenderable
{
    public virtual void Load()
    {
    }

    public virtual void Render(CommandList commands)
    {
    }

    public virtual void Update(double elapsed)
    {
    }

    public virtual void Unload()
    {
    }

    protected sealed override void OnLoad()
    {
        Load();
    }

    protected sealed override void OnUnload()
    {
        Unload();
    }

    protected sealed override void Destroy()
    {
        base.Destroy();
    }
}
