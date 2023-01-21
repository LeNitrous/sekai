// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;

namespace Sekai;

/// <summary>
/// A scriptable that allows repeated calls through its update method.
/// </summary>
[Processor<BehaviorProcessor>]
public abstract class Behavior : Scriptable
{
    /// <summary>
    /// Called every frame to perform updates.
    /// </summary>
    public abstract void Update();
}
