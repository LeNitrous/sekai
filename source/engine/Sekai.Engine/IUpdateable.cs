// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;

/// <summary>
/// An interface denoting a given object can be called in the main update thread.
/// </summary>
public interface IUpdateable
{
    /// <summary>
    /// Called in the update thread every frame.
    /// </summary>
    /// <param name="delta">The elapsed time between frames in milliseconds.</param>
    void Update(double delta);
}
