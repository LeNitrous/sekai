// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

public interface IInputContext
{
    /// <summary>
    /// A list of all available input devices.
    /// </summary>
    IReadOnlyList<IInputDevice> Available { get; }

    /// <summary>
    /// Called when the connection state of an input device changes.
    /// </summary>
    event Action<IInputDevice, bool> OnConnectionChanged;
}
