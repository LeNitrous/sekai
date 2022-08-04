// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Annotations;

namespace Sekai.Engine;

public abstract class GameSystem : ActivatableObject
{
    [Resolved]
    protected GameSystemCollection Systems = null!;
}
