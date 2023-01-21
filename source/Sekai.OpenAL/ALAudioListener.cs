// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.CompilerServices;
using Sekai.Audio;
using Silk.NET.OpenAL;

namespace Sekai.OpenAL;

internal sealed unsafe class ALAudioListener : NativeAudioListener
{
    public override float Volume
    {
        get
        {
            system.GetListenerProperty(ListenerFloat.Gain, out float value);
            return value;
        }
        set => system.SetListenerProperty(ListenerFloat.Gain, value);
    }

    public override Vector3 Velocity
    {
        get
        {
            system.GetListenerProperty(ListenerVector3.Velocity, out Vector3 value);
            return value;
        }
        set => system.SetListenerProperty(ListenerVector3.Velocity, value);
    }

    public override Vector3 Position
    {
        get
        {
            system.GetListenerProperty(ListenerVector3.Position, out Vector3 value);
            return value;
        }
        set => system.SetListenerProperty(ListenerVector3.Position, value);
    }

    public override ListenerOrientation Orientation
    {
        get
        {
            ListenerOrientation orientation = new();
            float* ptr = (float*)Unsafe.AsPointer(ref orientation);
            system.GetListenerProperty(ListenerFloatArray.Orientation, ptr);
            return orientation;
        }
        set => system.SetListenerProperty(ListenerFloatArray.Orientation, (float*)Unsafe.AsPointer(ref value));
    }

    private readonly ALAudioSystem system;

    public ALAudioListener(ALAudioSystem system)
    {
        this.system = system;
    }
}
