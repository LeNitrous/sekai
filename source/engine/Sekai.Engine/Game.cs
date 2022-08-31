// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Allocation;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : LoadableObject
{
    [Resolved]
    public SystemCollection<GameSystem> Systems = null!;
}
