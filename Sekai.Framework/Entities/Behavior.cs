// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Entities.Processors;

namespace Sekai.Framework.Entities;

/// <summary>
/// A <see cref="Component"/> that runs every frame through <see cref="Update"/>.
/// </summary>
[EntityProcessor(typeof(BehaviorProcessor))]
public abstract class Behavior : Component, IUpdateable
{
    public abstract void Update(double elapsed);
}
