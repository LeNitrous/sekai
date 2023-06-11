// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// A game host whose lifetime events are manually invoked.
/// </summary>
public sealed class ManualGameHost : Host
{
    public override IView? View => null;

    protected override void Run()
    {
    }

    public new void DoTick() => base.DoTick();

    public new void Exit() => base.Exit();

    public new void Pause() => base.Pause();

    public new void Resume() => base.Resume();
}
