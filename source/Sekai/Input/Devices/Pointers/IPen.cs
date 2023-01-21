// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System;

namespace Sekai.Input.Devices.Pointers;

/// <summary>
/// An interface representing a tablet pen.
/// </summary>
public interface IPen : IPointer
{
    /// <summary>
    /// A collection of <see cref="PenButton"/>s this pen supports.
    /// </summary>
    IReadOnlyList<PenButton> Buttons { get; }

    /// <summary>
    /// Called when a <see cref="PenButton"/> has been pressed down.
    /// </summary>
    event Action<IPen, PenButton>? OnButtonPressed;

    /// <summary>
    /// Called when a <see cref="PenButton"/> has been released.
    /// </summary>
    event Action<IPen, PenButton>? OnButtonRelease;
}
