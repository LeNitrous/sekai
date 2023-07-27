// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Input;

internal sealed class InputSource : IInputSource
{
    public IEnumerable<IInputDevice> Devices => sources.SelectMany(s => s.Devices);

    public event Action<IInputDevice, bool>? ConnectionChanged;

    private readonly IReadOnlyList<IInputSource> sources;

    public InputSource(IReadOnlyList<IInputSource> sources)
    {
        foreach (var source in sources)
        {
            source.ConnectionChanged += handleConnectionChanged;
        }

        this.sources = sources;
    }

    private void handleConnectionChanged(IInputDevice device, bool connected) => ConnectionChanged?.Invoke(device, connected);
}
