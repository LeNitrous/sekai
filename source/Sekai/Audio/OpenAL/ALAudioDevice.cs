// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenAL;

namespace Sekai.Audio.OpenAL;

internal sealed unsafe class ALAudioDevice : AudioDevice
{
    public override AudioListener Listener { get; }

    private bool isDisposed;
    private readonly AL AL;
    private readonly ALContext ALC;
    private readonly Device* device;
    private readonly Context* context;

    public ALAudioDevice()
    {
        AL = AL.GetApi(true);
        ALC = ALContext.GetApi(true);

        device = ALC.OpenDevice(null);
        context = ALC.CreateContext(device, null);
        Listener = new ALAudioListener(AL);

        ALC.MakeContextCurrent(context);
    }

    public override AudioBuffer CreateBuffer()
    {
        return new ALAudioBuffer(AL);
    }

    public override AudioSource CreateSource()
    {
        return new ALAudioSource(AL);
    }

    ~ALAudioDevice()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        ALC.DestroyContext(context);
        ALC.CloseDevice(device);
        ALC.Dispose();
        AL.Dispose();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
