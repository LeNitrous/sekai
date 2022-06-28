// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Extensions;
using Veldrid;

namespace Sekai.Framework.Graphics.Textures;

public class Sampler : GraphicsObject<Veldrid.Sampler>, IBindableResource
{
    public readonly SamplerAddressMode ModeU;
    public readonly SamplerAddressMode ModeV;
    public readonly SamplerAddressMode ModeW;
    public readonly SamplerFilter Filter;
    public readonly ComparisonKind? Comparison;
    public readonly int MaximumAnisoptropy;
    public readonly int MinimumLod;
    public readonly int MaximumLod;
    public readonly int LodBias;
    public readonly SamplerBorder Border;
    internal override Veldrid.Sampler Resource { get; }

    public Sampler(SamplerAddressMode modeU, SamplerAddressMode modeV, SamplerAddressMode modeW, SamplerFilter filter, ComparisonKind? comparison, int maximumAnisotropy, int minimumLod, int maximumLod, int lodBias, SamplerBorder border)
    {
        ModeU = modeU;
        ModeV = modeV;
        ModeW = modeW;
        Filter = filter;
        Comparison = comparison;
        MaximumAnisoptropy = maximumAnisotropy;
        MinimumLod = minimumLod;
        MaximumLod = maximumLod;
        LodBias = lodBias;
        Border = border;
        Resource = Context.Resources.CreateSampler(new SamplerDescription(ModeU.ToVeldrid(), ModeV.ToVeldrid(), ModeW.ToVeldrid(), Filter.ToVeldrid(), Comparison?.ToVeldrid(), (uint)MaximumAnisoptropy, (uint)MinimumLod, (uint)MaximumLod, LodBias, Border.ToVeldrid()));
    }

    BindableResource IBindableResource.Resource => Resource;
}
