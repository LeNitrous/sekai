// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// A game host that can be hosted as a service.
/// </summary>
public class HeadlessGameHost : Host
{
    public override IView? View => null;

    protected override void Run()
    {
        while (State != HostState.Exited)
        {
            DoTick();
        }
    }
}
