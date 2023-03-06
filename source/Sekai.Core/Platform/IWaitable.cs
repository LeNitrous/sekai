// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform;

/// <summary>
/// An interface for waitable objects.
/// </summary>
public interface IWaitable
{
    /// <summary>
    /// Blocking the calling thread and waits for an arbitrary amount of time.
    /// </summary>
    /// <param name="time">The duration of the wait.</param>
    void Wait(TimeSpan time);
}
