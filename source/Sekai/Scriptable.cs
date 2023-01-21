// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai;

/// <summary>
/// A component where it can have its own behavior defined.
/// </summary>
[Processor<ScriptableProcessor>]
public abstract class Scriptable : Component
{
    /// <summary>
    /// Called when the script has been attached on to the scene.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// Called when the script has been detached from the scene.
    /// </summary>
    public virtual void Unload()
    {
    }
}
