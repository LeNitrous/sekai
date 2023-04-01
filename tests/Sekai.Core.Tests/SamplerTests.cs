// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Graphics.Textures;

namespace Sekai.Core.Tests;

public class SamplerTests : GraphicsDeviceTest
{
    [Test]
    public void Create_Succeeds()
    {
        SamplerState sampler = null!;

        Assert.That(() => sampler = SamplerState.Create
        (
            Graphics,
            TextureAddressMode.Wrap,
            TextureAddressMode.Wrap,
            TextureAddressMode.Wrap,
            TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint,
            TextureBorderColor.Transparent,
            0,
            0,
            int.MaxValue,
            0
        ), Throws.Nothing);

        Assert.Multiple(() =>
        {
            Assert.That(sampler.AddressModeU, Is.EqualTo(TextureAddressMode.Wrap));
            Assert.That(sampler.AddressModeV, Is.EqualTo(TextureAddressMode.Wrap));
            Assert.That(sampler.AddressModeW, Is.EqualTo(TextureAddressMode.Wrap));
            Assert.That(sampler.Border, Is.EqualTo(TextureBorderColor.Transparent));
            Assert.That(sampler.Filter, Is.EqualTo(TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint));
            Assert.That(sampler.LodBias, Is.EqualTo(0));
            Assert.That(sampler.MinimumLod, Is.EqualTo(0));
            Assert.That(sampler.MaximumLod, Is.EqualTo(int.MaxValue));
            Assert.That(sampler.MaximumAnisotropy, Is.EqualTo(0));
        });

        sampler.Dispose();
    }

    [Test]
    public void Create_Fails()
    {
        Assert.That(() => SamplerState.Create
        (
            Graphics,
            TextureAddressMode.Wrap,
            TextureAddressMode.Wrap,
            TextureAddressMode.Wrap,
            TextureFilteringMode.MinPoint,
            TextureBorderColor.Transparent,
            0,
            0,
            int.MaxValue,
            0
        ), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }
}
