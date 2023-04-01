// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Extensions;

namespace Sekai.Graphics.Textures;

/// <summary>
/// Represents a graphics object that determines how values are sampled within a <see cref="Texture"/>.
/// </summary>
public sealed class SamplerState : IDisposable
{
    /// <summary>
    /// The level of detail bias.
    /// </summary>
    public int LodBias => samplerDescription.LodBias;

    /// <summary>
    /// The minimum level of detail.
    /// </summary>
    public int MinimumLod => (int)samplerDescription.MinimumLod;

    /// <summary>
    /// The maximum level of detail.
    /// </summary>
    public int MaximumLod => (int)samplerDescription.MaximumLod;

    /// <summary>
    /// The maximum anisotropy of the filter.
    /// </summary>
    /// <remarks>
    /// This property is only valid when <see cref="TextureFilteringMode.Anisotropic"/> is used.
    /// </remarks>
    public int MaximumAnisotropy => (int)samplerDescription.MaximumAnisotropy;

    /// <summary>
    /// The constant color sampled on the texture's borders.
    /// </summary>
    /// <remarks>
    /// This property is only used when <see cref="TextureAddressMode.Border"/> is used.
    /// </remarks>
    public TextureBorderColor Border => samplerDescription.BorderColor.AsBorderColor();

    /// <summary>
    /// The filter used for sampling the texture.
    /// </summary>
    public TextureFilteringMode Filter => samplerDescription.Filter.AsFilteringMode();

    /// <summary>
    /// The sampling address mode used on the U (or S) coordinate.
    /// </summary>
    public TextureAddressMode AddressModeU => samplerDescription.AddressModeU.AsAddressMode();

    /// <summary>
    /// The sampling address mode used on the V (or T) coordinate.
    /// </summary>
    public TextureAddressMode AddressModeV => samplerDescription.AddressModeV.AsAddressMode();

    /// <summary>
    /// The sampling address mode used on the W (or R) coordinate.
    /// </summary>
    public TextureAddressMode AddressModeW => samplerDescription.AddressModeW.AsAddressMode();

    private bool isDisposed;
    private readonly Veldrid.Sampler sampler;
    private readonly Veldrid.SamplerDescription samplerDescription;

    private SamplerState(Veldrid.Sampler sampler, Veldrid.SamplerDescription samplerDescription)
    {
        this.sampler = sampler;
        this.samplerDescription = samplerDescription;
    }

    /// <summary>
    /// Creates a new sampler state.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="addressModeU">The addressing mode for the U (or S) coordinate.</param>
    /// <param name="addressModeV">The addressing mode for the V (or R) coordinate.</param>
    /// <param name="addressModeW">The addressing mode for the W (or T) coordainte.</param>
    /// <param name="filter">The filtering mode.</param>
    /// <param name="color">The color to be sampled when <see cref="TextureAddressMode.Border"/> is used.</param>
    /// <param name="lodBias">The level of detail bias.</param>
    /// <param name="minLod">The minimum level of detail.</param>
    /// <param name="maxLod">The maximum level of detail.</param>
    /// <param name="maxAnisotropy">The maximumm anisotropy when <see cref="TextureFilteringMode.Anisotropic"/> is used.</param>
    /// <returns>A new sampler state.</returns>
    public static SamplerState Create(GraphicsDevice device, TextureAddressMode addressModeU, TextureAddressMode addressModeV, TextureAddressMode addressModeW, TextureFilteringMode filter, TextureBorderColor color, int lodBias, int minLod, int maxLod, int maxAnisotropy)
    {
        var descriptor = new Veldrid.SamplerDescription
        (
            addressModeU.AsAddressMode(),
            addressModeV.AsAddressMode(),
            addressModeW.AsAddressMode(),
            filter.AsVeldridFilter(),
            null,
            (uint)maxAnisotropy,
            (uint)minLod,
            (uint)maxLod,
            lodBias,
            color.AsVeldridBorder()
        );

        var sampler = device.Factory.CreateSampler(descriptor);

        return new(sampler, descriptor);
    }

    ~SamplerState()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        sampler.Dispose();

        isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
