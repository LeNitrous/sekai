// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Windowing;

/// <summary>
/// Determines whether a given object can restart.
/// </summary>
public interface IHasRestart
{
    /// <summary>
    /// Called when window requests a restart.
    /// </summary>
    event Action? Restart;
}
