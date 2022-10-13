// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;

public abstract class Behavior : Component, IUpdateable
{
    internal bool HasStarted = false;

    public virtual void Start()
    {
    }

    public abstract void Update(double elapsed);
}
