// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai;

/// <summary>
/// Logical components that have a startup method through <see cref="Start"/> which is called at the start of the frame.
/// </summary>
[Processor<ScriptProcessor>]
public abstract class Script : Component
{
    /// <summary>
    /// Gets whether this script has started or not.
    /// </summary>
    public bool HasStarted { get; private set; }

    /// <summary>
    /// Called once to perform custom startup logic.
    /// </summary>
    public abstract void Start();

    internal void Load()
    {
        if (HasStarted)
            return;

        Start();

        HasStarted = true;
    }
}
