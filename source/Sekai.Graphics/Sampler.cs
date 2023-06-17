// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Graphics;

public abstract class Sampler : IDisposable
{
    /// <inheritdoc cref="SamplerDescription.Filter"/>
    public abstract TextureFilter Filter { get; }

    /// <inheritdoc cref="SamplerDescription.AddressU"/>
    public abstract TextureAddress AddressU { get; }

    /// <inheritdoc cref="SamplerDescription.AddressV"/>
    public abstract TextureAddress AddressV { get; }

    /// <inheritdoc cref="SamplerDescription.AddressW"/>
    public abstract TextureAddress AddressW { get; }

    /// <inheritdoc cref="SamplerDescription.MaxAnisotropy"/>
    public abstract int MaxAnisotropy { get; }

    /// <inheritdoc cref="SamplerDescription.BorderColor"/>
    public abstract Color BorderColor { get; }

    /// <inheritdoc cref="SamplerDescription.LODBias"/>
    public abstract float LODBias { get; }

    /// <inheritdoc cref="SamplerDescription.MinimumLOD"/>
    public abstract float MinimumLOD { get; }

    /// <inheritdoc cref="SamplerDescription.MaximumLOD"/>
    public abstract float MaximumLOD { get; }

    public abstract void Dispose();
}
