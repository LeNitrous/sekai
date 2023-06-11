// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.CompilerServices;
using Silk.NET.OpenAL;

namespace Sekai.Audio.OpenAL;

internal sealed class ALAudioListener : AudioListener
{
    public override float Volume
    {
        get
        {
            AL.GetListenerProperty(ListenerFloat.Gain, out float value);
            return value;
        }
        set => AL.SetListenerProperty(ListenerFloat.Gain, value);
    }

    public override Vector3 Position
    {
        get
        {
            AL.GetListenerProperty(ListenerVector3.Position, out var value);
            return value;
        }
        set => AL.SetListenerProperty(ListenerVector3.Position, value);
    }

    public override Vector3 Velocity
    {
        get
        {
            AL.GetListenerProperty(ListenerVector3.Velocity, out var value);
            return value;
        }
        set => AL.SetListenerProperty(ListenerVector3.Velocity, value);
    }

    public override unsafe ListenerOrientation Orientation
    {
        get
        {
            ListenerOrientation value = default;
            AL.GetListenerProperty(ListenerFloatArray.Orientation, (float*)Unsafe.AsPointer(ref value));
            return value;
        }
        set => AL.SetListenerProperty(ListenerFloatArray.Orientation, (float*)Unsafe.AsPointer(ref value));
    }

#pragma warning disable IDE1006

    private readonly AL AL;

#pragma warning restore IDE1006

    public ALAudioListener(AL al)
    {
        AL = al;
    }

    public override void Dispose()
    {
    }
}
