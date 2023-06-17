// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.Creative;
using Silk.NET.OpenAL.Extensions.Enumeration;
using Silk.NET.OpenAL.Extensions.Soft;

namespace Sekai.Audio.OpenAL;

public sealed unsafe class ALAudioDevice : AudioDevice
{
    public override AudioAPI API => AudioAPI.OpenAL;
    public override Version Version { get; }
    public override AudioListener Listener { get; }
    public override string Device
    {
        get => getAudioDevice();
        set => setAudioDevice(value);
    }

    private bool isDisposed;
    private readonly Device* device;
    private readonly Context* context;

#pragma warning disable IDE1006

    private readonly AL AL;
    private readonly ALContext ALC;

#pragma warning restore IDE1006

    public ALAudioDevice()
    {
        AL = AL.GetApi(true);
        ALC = ALContext.GetApi(true);

        device = ALC.OpenDevice(null);
        context = ALC.CreateContext(device, null);
        Listener = new ALAudioListener(AL);

        int major = 0;
        int minor = 0;

        ALC.GetContextProperty(device, GetContextInteger.MajorVersion, 1, &major);
        ALC.GetContextProperty(device, GetContextInteger.MinorVersion, 1, &minor);

        Version = new Version(major, minor);

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

    public override void Process()
    {
        ALC.ProcessContext(context);
    }

    public override void Suspend()
    {
        ALC.SuspendContext(context);
    }

    private Enumeration? enumeration;
    private EnumerateAll? enumerateAll;
    private ReopenDevices? reopenDevices;

    public override IEnumerable<string> GetAvailableDevices()
    {
        if (enumerateAll is not null || ALC.TryGetExtension<EnumerateAll>(ALC.GetContextsDevice(context), out enumerateAll))
            return enumerateAll.GetStringList(GetEnumerateAllContextStringList.AllDevicesSpecifier);

        if (enumeration is not null || ALC.TryGetExtension<Enumeration>(ALC.GetContextsDevice(context), out enumeration))
            return enumeration.GetStringList(GetEnumerationContextStringList.DeviceSpecifiers);

        return Enumerable.Empty<string>();
    }

    private void setAudioDevice(string value)
    {
        if (reopenDevices is not null || AL.TryGetExtension<ReopenDevices>(out reopenDevices))
        {
            string? specifier = value == DEFAULT_AUDIO_DEVICE ? null : value;

            if (!reopenDevices.ReopenDevice(device, specifier, null))
            {
                throw new InvalidOperationException($"An OpenAL exception has occured: {AL.GetError()}");
            }
        }
    }

    private string getAudioDevice()
    {
        return ALC.GetContextProperty(ALC.GetContextsDevice(context), GetContextString.DeviceSpecifier);
    }
}
