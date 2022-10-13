// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;

namespace Sekai.Engine;

public abstract class SceneSystem : FrameworkObject, IGameSystem
{
    public bool Enabled { get; set; } = true;

    public Scene Scene { get; internal set; } = null!;

    public virtual void Initialize()
    {
    }
}
