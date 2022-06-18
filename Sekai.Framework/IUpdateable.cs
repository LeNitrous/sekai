// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework;

/// <summary>
/// An interface denoting a given object can be called in the update thread.
/// </summary>
public interface IUpdateable
{
    /// <summary>
    /// Called in the update thread every frame.
    /// </summary>
    /// <param name="elapsed">The elapsed time between frames in milliseconds.</param>
    void Update(double elapsed);
}
