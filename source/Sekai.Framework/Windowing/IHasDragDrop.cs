// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Windowing;

/// <summary>
/// Determines whether a given object is capable of content dropping.
/// </summary>
public interface IHasDragDrop
{
    /// <summary>
    /// Called when content has been dropped on the window.
    /// </summary>
    event Action<string[]> Dropped;
}
