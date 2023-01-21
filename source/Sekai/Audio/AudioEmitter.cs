// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Processors;
using Sekai.Rendering;
using Sekai.Scenes;

namespace Sekai.Audio;

/// <summary>
/// A component that can emit audio.
/// </summary>
[Processor<AudioEmitterProcessor>]
public class AudioEmitter : Component
{
    /// <inheritdoc cref="IAudioController.Loop"/>
    public bool Loop
    {
        get => Controller?.Loop ?? false;
        set
        {
            if (Controller is not null)
                Controller.Loop = value;
        }
    }

    /// <inheritdoc cref="IAudioController.Position"/>
    public TimeSpan Position
    {
        get => Controller?.Position ?? TimeSpan.Zero;
        set
        {
            if (Controller is not null)
                Controller.Position = value;
        }
    }

    /// <inheritdoc cref="IAudioController.Duration"/>
    public TimeSpan Duration => Controller?.Duration ?? TimeSpan.Zero;

    /// <inheritdoc cref="IAudioController.Available"/>
    public TimeSpan Available => Controller?.Available ?? TimeSpan.Zero;

    /// <summary>
    /// The audio stream to play.
    /// </summary>
    public AudioStream? Stream { get; set; }

    /// <summary>
    /// The emitter's transform.
    /// </summary>
    internal Transform? Transform
    {
        get
        {
            if (transform2D is not null && transform3D is not null)
                throw new InvalidOperationException(@"An audio emitter cannot have both a 2D and 3D transform at the same time.");

            if (transform2D is not null)
                return transform2D;

            if (transform3D is not null)
                return transform3D;

            return null;
        }
    }

    /// <summary>
    /// The emitter's audio controller.
    /// </summary>
    internal IAudioController? Controller { get; set; }

    [Bind]
    private Transform2D? transform2D { get; set; }

    [Bind]
    private Transform3D? transform3D { get; set; }
}
