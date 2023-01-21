// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Allocation;

namespace Sekai.Audio;

public sealed class AudioContext : DependencyObject
{
    private readonly ConcurrentQueue<Action> controllerMutationQueue = new();
    private readonly List<LeasedAudioController> controllers = new();
    private readonly ObjectPool<NativeAudioSource> sourcePool; 
    private readonly ObjectPool<NativeAudioBuffer> bufferPool;
    private readonly NativeAudioListener nativeListener;

    internal const int MAX_SOURCES = 500;
    internal const int MAX_BUFFERS = 500;
    internal const int MAX_SOURCE_THRESHOLD = 5120;
    internal const int MAX_BUFFER_ALLOC = 65536;
    internal const int MAX_BUFFER_PER_SOURCE = 4;

    /// <summary>
    /// The current audio listener.
    /// </summary>
    internal IAudioListener? Current { get; private set; }

    [Resolved]
    private AudioSystem audio { get; set; } = null!;

    public AudioContext()
    {
        nativeListener = audio.CreateListener();
        sourcePool = new ObjectPool<NativeAudioSource>(MAX_SOURCES, new AudioSourcePoolingStrategy(audio));
        bufferPool = new ObjectPool<NativeAudioBuffer>(MAX_BUFFERS, new AudioBufferPoolingStrategy(audio));
    }

    /// <summary>
    /// Makes the listener current.
    /// </summary>
    /// <param name="listener">The listener to set.</param>
    internal void MakeCurrent(IAudioListener listener)
    {
        if (Current == listener)
            return;

        nativeListener.Position = listener.Position;
        nativeListener.Velocity = listener.Velocity;
        nativeListener.Orientation = listener.Orientation;
    }

    /// <summary>
    /// Clears the current listener.
    /// </summary>
    /// <param name="listener">The listener invoking this method.</param>
    internal void ClearCurrent(IAudioListener listener)
    {
        if (Current != listener)
            return;

        nativeListener.Position = Vector3.Zero;
        nativeListener.Velocity = Vector3.Zero;
        nativeListener.Orientation = ListenerOrientation.Default;
    }

    /// <summary>
    /// Gets an audio controller for the provided stream.
    /// </summary>
    /// <param name="stream">The stream to use for the controller.</param>
    /// <returns>An audio controller.</returns>
    internal IAudioController GetController(AudioStream stream)
    {
        var controller = new LeasedAudioController(new AudioStream(stream), sourcePool, bufferPool, removeController);
        addController(controller);
        return controller;
    }

    private void addController(LeasedAudioController controller)
        => controllerMutationQueue.Enqueue(() => controllers.Add(controller));

    private void removeController(LeasedAudioController controller)
        => controllerMutationQueue.Enqueue(() => controllers.Remove(controller));

    internal void Update()
    {
        while (controllerMutationQueue.TryDequeue(out var action))
            action();

        foreach (var controller in controllers)
            controller.Update();
    }

    protected override void Destroy()
    {
        foreach (var controller in controllers)
            controller.Dispose();

        controllers.Clear();
        sourcePool.Dispose();
        bufferPool.Dispose();
    }

    private class AudioSourcePoolingStrategy : ObjectPoolingStrategy<NativeAudioSource>
    {
        private readonly AudioSystem audio;

        public AudioSourcePoolingStrategy(AudioSystem audio)
        {
            this.audio = audio;
        }

        public override NativeAudioSource Create() => audio.CreateSource();

        public override bool Return(NativeAudioSource obj) => true;
    }

    private class AudioBufferPoolingStrategy : ObjectPoolingStrategy<NativeAudioBuffer>
    {
        private readonly AudioSystem audio;

        public AudioBufferPoolingStrategy(AudioSystem audio)
        {
            this.audio = audio;
        }

        public override NativeAudioBuffer Create() => audio.CreateBuffer();

        public override bool Return(NativeAudioBuffer obj) => true;
    }
}
