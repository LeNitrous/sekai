// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.Headless;

internal class HeadlessInputContext : IInputContext
{
    public IReadOnlyList<IInputDevice> Available { get; } = Array.Empty<IInputDevice>();
    public event Action<IInputDevice, bool> OnConnectionChanged = null!;
}
