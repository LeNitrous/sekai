// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Audio;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.Creative;
using Silk.NET.OpenAL.Extensions.Enumeration;

namespace Sekai.OpenAL;

internal sealed unsafe class ALAudioSystem : AudioSystem
{
    public override string Name { get; } = @"OpenAL";

    public override Version Version { get; }

    public override IReadOnlyList<string> Extensions { get; }

    public override string Device
    {
        get => deviceName ?? DEFAULT_DEVICE;
        set
        {
            if (deviceName == value)
                return;

            updateAvailableDevices();

            if (!devices.TryGetValue(value, out string? name))
                throw new ArgumentOutOfRangeException(nameof(value));

            deviceName = name;
        }
    }

    public override IEnumerable<string> Devices
    {
        get
        {
            updateAvailableDevices();
            return devices.Keys;
        }
    }

    private string? deviceName;
    private ALAudioListener? listener;
    private readonly AL al;
    private readonly ALContext alc;
    private readonly Device* device;
    private readonly Context* context;
    private readonly Dictionary<string, string?> devices = new();
    private readonly Dictionary<uint, NativeAudioBuffer> buffers = new();

    public ALAudioSystem()
    {
        al = AL.GetApi(true);
        alc = ALContext.GetApi(true);

        device = alc.OpenDevice(deviceName);
        context = alc.CreateContext(device, null);
        alc.MakeContextCurrent(context);

        int major = 0;
        int minor = 0;
        alc.GetContextProperty(device, GetContextInteger.MajorVersion, 1, &major);
        alc.GetContextProperty(device, GetContextInteger.MinorVersion, 1, &minor);
        Version = new(major, minor);

        Extensions = alc.GetContextProperty(device, GetContextString.Extensions).Split(' ');

        updateAvailableDevices();
    }

    public override NativeAudioBuffer CreateBuffer()
    {
        uint bufferId = al.GenBuffer();
        var buffer = new ALAudioBuffer(this, bufferId);
        buffers.Add(bufferId, buffer);
        return buffer;
    }

    public override NativeAudioSource CreateSource() => new ALAudioSource(this, al.GenSource());

    public override NativeAudioListener CreateListener() => listener ??= new ALAudioListener(this);

    private Enumeration? enumeration;
    private EnumerateAll? enumerateAll;

    private IEnumerable<string> getAvailableDevices()
    {
        if (enumerateAll is not null || alc.TryGetExtension<EnumerateAll>(null, out enumerateAll))
            return enumerateAll.GetStringList(GetEnumerateAllContextStringList.AllDevicesSpecifier);

        if (enumeration is not null || alc.TryGetExtension<Enumeration>(null, out enumeration))
            return enumeration.GetStringList(GetEnumerationContextStringList.DeviceSpecifiers);

        return Enumerable.Empty<string>();
    }

    private void updateAvailableDevices()
    {
        devices.Clear();
        devices.Add(DEFAULT_DEVICE, null);

        foreach (string deviceName in getAvailableDevices())
            devices.Add(deviceName, deviceName);
    }

    internal void SetBufferData(ALAudioBuffer buffer, nint data, int size, AudioFormat format, int sampleRate)
    {
        var bufferFmt = format switch
        {
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            _ => throw new NotSupportedException(),
        };

        al.BufferData(buffer, bufferFmt, (void*)data, size, sampleRate);
        throwIfError();
    }

    internal void GetBufferProperty(ALAudioBuffer buffer, BufferFloat prop, out float value)
    {
        al.GetBufferProperty(buffer, prop, out value);
        throwIfError();
    }

    internal void GetBufferProperty(ALAudioBuffer buffer, BufferVector3 prop, out Vector3 value)
    {
        al.GetBufferProperty(buffer, prop, out value);
        throwIfError();
    }

    internal void GetBufferProperty(ALAudioBuffer buffer, BufferVector3 prop, out float x, out float y, out float z)
    {
        al.GetBufferProperty(buffer, prop, out x, out y, out z);
        throwIfError();
    }

    internal void GetBufferProperty(ALAudioBuffer buffer, GetBufferInteger prop, out int value)
        {
        al.GetBufferProperty(buffer, prop, out value);
        throwIfError();
    }

    internal void GetBufferProperty(ALAudioBuffer buffer, GetBufferInteger prop, out int value1, out int value2, out int value3)
    {
        al.GetBufferProperty(buffer, prop, out value1, out value2, out value3);
        throwIfError();
    }

    internal void DestroyBuffer(ALAudioBuffer buffer)
    {
        buffers.Remove(buffer);
        al.DeleteBuffer(buffer);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceFloat prop, float value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceBoolean prop, bool value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, int value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, int* value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, uint value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, uint* value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceVector3 prop, float* value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceVector3 prop, in Vector3 value)
    {
        al.SetSourceProperty(source, prop, value);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceVector3 prop, float value1, float value2, float value3)
    {
        al.SetSourceProperty(source, prop, value1, value2, value3);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, int value1, int value2, int value3)
    {
        al.SetSourceProperty(source, prop, value1, value2, value3);
        throwIfError();
    }

    internal void SetSourceProperty(ALAudioSource source, SourceInteger prop, uint value1, uint value2, uint value3)
    {
        al.SetSourceProperty(source, prop, value1, value2, value3);
        throwIfError();
    }

    internal void GetSourceProperty(ALAudioSource source, GetSourceInteger prop, out int value)
    {
        al.GetSourceProperty(source, prop, out value);
        throwIfError();
    }

    internal void GetSourceProperty(ALAudioSource source, SourceBoolean prop, out bool value)
    {
        al.GetSourceProperty(source, prop, out value);
        throwIfError();
    }

    internal void GetSourceProperty(ALAudioSource source, SourceFloat prop, out float value)
    {
        al.GetSourceProperty(source, prop, out value);
        throwIfError();
    }

    internal void GetSourceProperty(ALAudioSource source, SourceVector3 prop, out Vector3 value)
    {
        al.GetSourceProperty(source, prop, out value);
        throwIfError();
    }

    internal void SourcePlay(ALAudioSource source)
    {
        al.SourcePlay(source);
        throwIfError();
    }

    internal void SourceStop(ALAudioSource source)
    {
        al.SourceStop(source);
        throwIfError();
    }

    internal void SourcePause(ALAudioSource source)
    {
        al.SourcePause(source);
        throwIfError();
    }

    internal void SourceRewind(ALAudioSource source)
    {
        al.SourceRewind(source);
        throwIfError();
    }

    internal void SourceEnqueue(ALAudioSource source, NativeAudioBuffer buffer)
    {
        Span<uint> bufferIds = stackalloc uint[1];
        bufferIds[0] = (ALAudioBuffer)buffer;

        fixed (uint* ptr = bufferIds)
        {
            al.SourceQueueBuffers(source, 1, ptr);
            throwIfError();
        }
    }

    internal void SourceClear(ALAudioSource source)
    {
        GetSourceProperty(source, GetSourceInteger.BuffersProcessed, out int processedCount);

        if (processedCount <= 0)
            return;

        Span<uint> bufferIds = stackalloc uint[processedCount];

        fixed (uint* bufferPtr = bufferIds)
        {
            al.SourceUnqueueBuffers(source, processedCount, bufferPtr);
            throwIfError();
        }
    }

    internal NativeAudioBuffer SourceDequeue(ALAudioSource source)
    {
        GetSourceProperty(source, GetSourceInteger.BuffersProcessed, out int processedCount);

        if (processedCount <= 0)
            throw new InvalidOperationException(@"There are no buffers to dequeue.");

        Span<uint> bufferIds = stackalloc uint[1];

        fixed (uint* bufferPtr = bufferIds)
        {
            al.SourceUnqueueBuffers(source, 1, bufferPtr);
            throwIfError();
        }

        if (!buffers.TryGetValue(bufferIds[0], out var buffer))
            throw new InvalidOperationException(@"Failed to dequeue buffer.");

        return buffer;
    }

    internal void DestroySource(ALAudioSource source)
    {
        al.DeleteSource(source);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerFloat prop, float value)
    {
        al.SetListenerProperty(prop, value);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerFloatArray prop, float* value)
    {
        al.SetListenerProperty(prop, value);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerInteger prop, int value)
    {
        al.SetListenerProperty(prop, value);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerInteger prop, int* value)
    {
        al.SetListenerProperty(prop, value);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerVector3 prop, in Vector3 value)
    {
        al.SetListenerProperty(prop, value);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerInteger prop, int value1, int value2, int value3)
    {
        al.SetListenerProperty(prop, value1, value2, value3);
        throwIfError();
    }

    internal void SetListenerProperty(ListenerVector3 prop, float value1, float value2, float value3)
    {
        al.SetListenerProperty(prop, value1, value2, value3);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerFloat prop, out float value)
    {
        al.GetListenerProperty(prop, out value);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerFloatArray prop, float* value)
    {
        al.GetListenerProperty(prop, value);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerInteger prop, out int value)
    {
        al.GetListenerProperty(prop, out value);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerInteger prop, int* value)
    {
        al.GetListenerProperty(prop, value);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerVector3 prop, out Vector3 value)
    {
        al.GetListenerProperty(prop, out value);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerVector3 prop, out float value1, out float value2, out float value3)
    {
        al.GetListenerProperty(prop, out value1, out value2, out value3);
        throwIfError();
    }

    internal void GetListenerProperty(ListenerInteger prop, out int value1, out int value2, out int value3)
    {
        al.GetListenerProperty(prop, out value1, out value2, out value3);
        throwIfError();
    }

    private void throwIfError()
    {
        var error = al.GetError();

        if (error != AudioError.NoError)
            throw new ALException(error);
    }

    protected override void Dispose(bool disposing)
    {
        enumeration?.Dispose();
        enumerateAll?.Dispose();
        alc.DestroyContext(context);
        alc.CloseDevice(device);
        alc.Dispose();
        al.Dispose();
    }
}

public class ALException : Exception
{
    public ALException(AudioError error)
        : base($@"An error has occured in OpenAL (Error Code: ""{error}"").")
    {
    }
}
