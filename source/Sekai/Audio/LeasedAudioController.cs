// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Allocation;

namespace Sekai.Audio;

/// <summary>
/// A leased audio controller provided by the <see cref="AudioContext"/>.
/// </summary>
internal sealed class LeasedAudioController : FrameworkObject, IAudioController
{
    public TimeSpan Position
    {
        get => bytesToTimeSpan(processed);
        set => seek(value);
    }

    public Vector3 PositionSpatial
    {
        get => source.Position;
        set => source.Position = value;
    }

    public bool Loop { get; set; }

    public TimeSpan Duration => bytesToTimeSpan((byte)stream.Length);

    public TimeSpan Available => bytesToTimeSpan(allocated);

    public bool IsPlaying => source.State == AudioSourceState.Playing;

    private int allocated;
    private int processed;
    private readonly AudioStream stream;
    private readonly NativeAudioSource source;
    private readonly NativeAudioBuffer[] buffers = new NativeAudioBuffer[AudioContext.MAX_BUFFER_PER_SOURCE];
    private readonly ObjectPool<NativeAudioSource> sourcePool;
    private readonly ObjectPool<NativeAudioBuffer> bufferPool;
    private readonly Action<LeasedAudioController> removeControllerAction;

    public LeasedAudioController(AudioStream stream, ObjectPool<NativeAudioSource> sourcePool, ObjectPool<NativeAudioBuffer> bufferPool, Action<LeasedAudioController> removeControllerAction)
    {
        this.stream = stream;
        this.sourcePool = sourcePool;
        this.bufferPool = bufferPool;
        this.removeControllerAction = removeControllerAction;

        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = bufferPool.Get();

        source = sourcePool.Get();
    }

    public void Play()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        seek(TimeSpan.Zero);

        foreach (var buffer in buffers)
        {
            if (!allocate(buffer))
            {
                source.Loop = Loop;
                break;
            }

            source.Enqueue(buffer);
        }

        source.Play();
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (source.State != AudioSourceState.Playing)
            return;

        seek(TimeSpan.Zero);
    }

    public void Pause()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (source.State != AudioSourceState.Playing)
            return;

        source.Pause();
    }

    public void Resume()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (source.State != AudioSourceState.Paused)
            return;

        source.Play();
    }

    public void Update()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (source.State != AudioSourceState.Playing)
            return;

        while (source.TryDequeue(out var buffer))
        {
            if (!allocate(buffer))
                break;

            source.Enqueue(buffer);
        }
    }

    private void seek(TimeSpan position)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        stop();
        stream.Position = processed = allocated = timeSpanToPosition(position);
    }

    private void stop()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        source.Stop();
        source.Clear();
    }

    private unsafe bool allocate(NativeAudioBuffer buffer)
    {
        Span<byte> data = stackalloc byte[AudioContext.MAX_BUFFER_ALLOC];
        int read = stream.Read(data);

        if (read <= 0)
            return false;

        fixed (byte* ptr = data)
            buffer.SetData((nint)ptr, read, stream.Format, stream.SampleRate);

        allocated += buffer.Size;

        return true;
    }

    protected override void Destroy()
    {
        stream.Dispose();
        stop();

        for (int i = 0; i < buffers.Length; i++)
            bufferPool.Return(buffers[i]);

        sourcePool.Return(source);

        removeControllerAction(this);
    }

    private TimeSpan bytesToTimeSpan(int byteCount)
        => TimeSpan.FromSeconds(byteCount / (stream.SampleRate * stream.Format.GetChannelCount() * (stream.Format.GetBitsPerSample() / 8)));

    private int timeSpanToPosition(TimeSpan time)
        => (int)time.TotalSeconds * stream.SampleRate * stream.Format.GetChannelCount() * (stream.Format.GetBitsPerSample() / 8);
}
