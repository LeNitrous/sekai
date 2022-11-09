// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Threading;

public abstract class UpdateThread
{
    public abstract void Update(double time);
    public abstract void FixedUpdate();
}
