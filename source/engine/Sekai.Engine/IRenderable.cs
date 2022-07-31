// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;

/// <summary>
/// An interface denoting a given object can be called in the main render thread.
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// Called every frame in the main render thread.
    /// </summary>
    /// <param name="commands">The main command list used for graphics commands.</param>
    void Render();
}
