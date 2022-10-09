// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.Dummy;

internal class DummyInputContext : IInputContext
{
    public IReadOnlyList<IInputDevice> Available { get; } = Array.Empty<IInputDevice>();
    public event Action<IInputDevice, bool> OnConnectionChanged = null!;
}
