// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Annotations;

namespace Sekai.Engine;

public abstract class SceneSystem : ActivatableObject
{
    [Resolved]
    public Scene Scene = null!;

    [Resolved]
    public SystemCollection<SceneSystem> Systems = null!;
}
