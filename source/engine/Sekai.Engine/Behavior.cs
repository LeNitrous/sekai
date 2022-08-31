// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;

public abstract class Behavior : Component, IUpdateable
{
    public abstract void Update(double elapsed);
}
