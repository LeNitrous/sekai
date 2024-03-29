// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Sekai.Mathematics;

namespace Sekai.Headless;

internal sealed class HeadlessSampler : Sampler
{
    public override TextureFilter Filter { get; }

    public override TextureAddress AddressU { get; }

    public override TextureAddress AddressV { get; }

    public override TextureAddress AddressW { get; }

    public override int MaxAnisotropy { get; }

    public override Color BorderColor { get; }

    public override float LODBias { get; }

    public override float MinimumLOD { get; }

    public override float MaximumLOD { get; }

    public HeadlessSampler(SamplerDescription description)
    {
        Filter = description.Filter;
        AddressU = description.AddressU;
        AddressV = description.AddressV;
        AddressW = description.AddressW;
        MaxAnisotropy = description.MaxAnisotropy;
        BorderColor = description.BorderColor;
        LODBias = description.LODBias;
        MinimumLOD = description.MaximumLOD;
        MaximumLOD = description.MaximumLOD;
    }

    public override void Dispose()
    {
    }
}
