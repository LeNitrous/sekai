// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Sekai.Mathematics;
using Silk.NET.OpenAL;

namespace Sekai.OpenAL;

internal unsafe class ALAudioSystem : FrameworkObject, IAudioSystem
{
    public float Gain { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 Orientation { get; set; }

    internal readonly AL AL;
    internal readonly ALContext ALC;

    public ALAudioSystem()
    {
        AL = AL.GetApi();
        ALC = ALContext.GetApi();
        var dev = ALC.OpenDevice(null);
        var con = ALC.CreateContext(dev, null);
        ALC.MakeContextCurrent(con);
    }

    public INativeAudioBuffer CreateBuffer() => new ALAudioBuffer(this);

    protected override void Destroy()
    {
        AL.Dispose();
        ALC.Dispose();
    }
}
