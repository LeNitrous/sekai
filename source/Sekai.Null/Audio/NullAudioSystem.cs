// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Audio;

namespace Sekai.Null.Audio;

internal class NullAudioSystem : AudioSystem
{
    public override string Device
    {
        get => devices[0];
        set { }
    }

    public override IEnumerable<string> Devices => devices;

    public override string Name { get; } = @"Null";

    public override Version Version { get; } = new Version();

    public override IReadOnlyList<string> Extensions { get; } = Array.Empty<string>();

    private readonly string[] devices = new string[] { @"Default" };

    public override NativeAudioBuffer CreateBuffer() => new NullAudioBuffer();

    public override NativeAudioSource CreateSource() => new NullAudioSource();

    public override NativeAudioListener CreateListener() => new NullAudioListener();
}
