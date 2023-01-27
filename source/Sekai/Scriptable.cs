// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading.Tasks;
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
    /// Called when the script has been attached to the scene.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// The asynchronous version of <see cref="Load"/>. Called when the script has been attached to the scene.
    /// </summary>
    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the script has been detached from the scene.
    /// </summary>
    public virtual void Unload()
    {
    }

    /// <summary>
    /// The asynchronous version of <see cref="UnloadAsync"/>. Called when the script has been detached from the scene.
    /// </summary>
    public virtual Task UnloadAsync()
    {
        return Task.CompletedTask;
    }
}
