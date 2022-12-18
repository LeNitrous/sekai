// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;

namespace Sekai;

/// <summary>
/// Logical scripts that have an <see cref="Update"/> method called every frame.
/// </summary>
[Processor<BehaviorProcessor>]
public abstract class Behavior : Script
{
    /// <summary>
    /// Called every frame to perform custom logic.
    /// </summary>
    /// <param name="delta">Time taken between frames.</param>
    public abstract void Update(double delta);
}
