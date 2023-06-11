// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

public sealed class InputManager
{
    private readonly IInputSource source;

    internal InputManager(IInputSource source)
    {
        this.source = source;
    }

    internal void Update()
    {
        foreach (var data in source.PumpEvents())
        {
            // TODO: Update state...
        }
    }
}
