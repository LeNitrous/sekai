// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Allocation;

namespace Sekai.Audio;

public sealed class AudioContext : DisposableObject
{
    /// <summary>
    /// Gets the audio system's name.
    /// </summary>
    public string System => system.Name;

    /// <summary>
    /// Gets the audio system's version.
    /// </summary>
    public Version Version => system.Version;

    /// <summary>
    /// Gets or sets the current audio device.
    /// </summary>
    public string Device
    {
        get => system.Device;
        set => system.Device = value;
    }

    /// <summary>
    /// Gets an enumeration of available audio devices.
    /// </summary>
    public IEnumerable<string> Devices => system.Devices;

    /// <summary>
    /// Gets a list of all available audio system extensions.
    /// </summary>
    public IReadOnlyList<string> Extensions => system.Extensions;

    private readonly ConcurrentQueue<Action> controllerMutationQueue = new();
    private readonly List<LeasedAudioController> controllers = new();
    private readonly ObjectPool<NativeAudioSource> sourcePool; 
    private readonly ObjectPool<NativeAudioBuffer> bufferPool;
    private readonly NativeAudioListener nativeListener;
    private readonly AudioSystem system;

    internal const int MAX_SOURCES = 500;
    internal const int MAX_BUFFERS = 500;
    internal const int MAX_SOURCE_THRESHOLD = 5120;
    internal const int MAX_BUFFER_ALLOC = 65536;
    internal const int MAX_BUFFER_PER_SOURCE = 4;

    /// <summary>
    /// The current audio listener.
    /// </summary>
    internal IAudioListener Current { get; private set; } = DefaultAudioListener.Default;

    internal AudioContext(AudioSystem system)
    {
        nativeListener = system.CreateListener();
        sourcePool = new ObjectPool<NativeAudioSource>(MAX_SOURCES, new AudioSourcePoolingStrategy(system));
        bufferPool = new ObjectPool<NativeAudioBuffer>(MAX_BUFFERS, new AudioBufferPoolingStrategy(system));
        this.system = system;
    }

    /// <summary>
    /// Makes the listener current.
    /// </summary>
    /// <param name="listener">The listener to set.</param>
    internal void MakeCurrent(IAudioListener listener)
    {
        if (Current == listener)
            return;

        Current = listener;
        nativeListener.Position = Current.Position;
        nativeListener.Velocity = Current.Velocity;
        nativeListener.Orientation = Current.Orientation;
    }

    /// <summary>
    /// Clears the current listener.
    /// </summary>
    /// <param name="listener">The listener invoking this method.</param>
    internal void ClearCurrent(IAudioListener listener)
    {
        if (Current != listener)
            return;

        MakeCurrent(DefaultAudioListener.Default);
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

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var controller in controllers)
            controller.Dispose();

        controllers.Clear();
        sourcePool.Dispose();
        bufferPool.Dispose();
    }

    private readonly struct DefaultAudioListener : IAudioListener
    {
        public static readonly DefaultAudioListener Default = new();
        public bool IsCurrent => true;
        public Vector3 Position => Vector3.Zero;
        public Vector3 Velocity => Vector3.Zero;
        public ListenerOrientation Orientation => ListenerOrientation.Default;
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
