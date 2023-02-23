// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;
using Sekai.Audio;

namespace Sekai.Processors;

internal sealed partial class AudioEmitterProcessor : Processor<AudioEmitter>
{
    [Resolved]
    private AudioContext audio { get; set; } = null!;

    protected override void OnComponentAttach(AudioEmitter emitter)
    {
        if (emitter.Stream is not null)
            emitter.Controller = audio.GetController(emitter.Stream);
    }

    protected override void OnComponentDetach(AudioEmitter emitter)
    {
        emitter.Controller?.Dispose();
        emitter.Controller = null;
    }

    protected override void Update(AudioEmitter emitter)
    {
        if (emitter.Controller is null || emitter.Transform is null)
            return;

        emitter.Controller.PositionSpatial = emitter.Transform.PositionMatrix.Translation;
    }
}
