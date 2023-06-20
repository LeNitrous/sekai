// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Input;

namespace Sekai.Headless.Input;

internal sealed class DummyInputContext : IInputContext
{
    public IEnumerable<IInputDevice> Devices => Enumerable.Empty<IInputDevice>();

#pragma warning disable IDE0067

    public event Action<IInputDevice, bool>? ConnectionChanged;

#pragma warning restore IDE0067

    public void Dispose()
    {
    }
}
