// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Audio;

namespace Sekai.Headless.Audio;

public sealed class DummyAudioDevice : AudioDevice
{
    public override AudioAPI API => AudioAPI.Dummy;
    public override Version Version { get; } = new();
    public override string Device
    {
        get => dummy_device;
        set { }
    }

    public override AudioListener Listener { get; } = new DummyAudioListener();

    private const string dummy_device = "Dummy";

    public override AudioBuffer CreateBuffer()
    {
        return new DummyAudioBuffer();
    }

    public override AudioSource CreateSource()
    {
        return new DummyAudioSource();
    }

    public override void Dispose()
    {
    }

    public override IEnumerable<string> GetAvailableDevices()
    {
        throw new NotImplementedException();
    }

    public override void Process()
    {
        throw new NotImplementedException();
    }

    public override void Suspend()
    {
        throw new NotImplementedException();
    }
}
