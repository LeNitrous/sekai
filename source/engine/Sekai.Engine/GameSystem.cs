// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;

namespace Sekai.Engine;

public abstract class GameSystem : FrameworkObject, IGameSystem
{
    public bool Enabled { get; set; } = true;
}
