// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Platform.Windowing;

/// <summary>
/// Determines whether a given object is suspendable.
/// </summary>
public interface ISuspendable
{
    /// <summary>
    /// Called when the window has resumed to foreground.
    /// </summary>
    event Action? Resume;

    /// <summary>
    /// Called when the window has suspended into background.
    /// </summary>
    event Action? Suspend;
}
