// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Windowing;

/// <summary>
/// Determines that an object can have window icons.
/// </summary>
public interface IHasIcon
{
    /// <summary>
    /// Sets the window's icons.
    /// </summary>
    /// <param name="icons">The icons to use.</param>
    void SetWindowIcon(ReadOnlySpan<Icon> icons);
}
