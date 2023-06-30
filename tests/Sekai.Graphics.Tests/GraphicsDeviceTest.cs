// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Sekai.Graphics.Tests;

public abstract class GraphicsDeviceTest<T> : GraphicsTest<T>
    where T : GraphicsDeviceCreator, new()
{
    [TestCase(64u, BufferType.Uniform, true)]
    [TestCase(64u, BufferType.Uniform, false)]
    public void CreateBuffer_Succeeds(uint size, BufferType type, bool dynamic)
    {
        using var buffer = Device.CreateBuffer(type, size, dynamic);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.Capacity, Is.EqualTo(size));
            Assert.That(buffer.Type, Is.EqualTo(type));
            Assert.That(buffer.Dynamic, Is.EqualTo(dynamic));
        });
    }

    [TestCase(2u, BufferType.Uniform, true)]
    [TestCase(2u, BufferType.Uniform, false)]
    public void CreateBuffer_T_Succeeds(uint count, BufferType type, bool dynamic)
    {
        using var buffer = Device.CreateBuffer<int>(type, count, dynamic);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.Capacity, Is.EqualTo(Unsafe.SizeOf<int>() * count));
            Assert.That(buffer.Type, Is.EqualTo(type));
            Assert.That(buffer.Dynamic, Is.EqualTo(dynamic));
        });
    }

    [Test]
    public void CreateTexture_Succeeds()
    {
        var descriptor = new TextureDescription
        {
            Count = TextureSampleCount.Count1,
            Depth = 1,
            Format = PixelFormat.R8G8B8A8_UNorm,
            Height = 1,
            Layers = 1,
            Levels = 1,
            Type = TextureType.Texture2D,
            Usage = TextureUsage.Resource,
            Width = 1,
        };

        using var texture = Device.CreateTexture(descriptor);

        Assert.Multiple(() =>
        {
            Assert.That(texture.Count, Is.EqualTo(descriptor.Count));
            Assert.That(texture.Depth, Is.EqualTo(descriptor.Depth));
            Assert.That(texture.Format, Is.EqualTo(descriptor.Format));
            Assert.That(texture.Height, Is.EqualTo(descriptor.Height));
            Assert.That(texture.Layers, Is.EqualTo(descriptor.Layers));
            Assert.That(texture.Levels, Is.EqualTo(descriptor.Levels));
            Assert.That(texture.Type, Is.EqualTo(descriptor.Type));
            Assert.That(texture.Usage, Is.EqualTo(descriptor.Usage));
            Assert.That(texture.Width, Is.EqualTo(descriptor.Width));
        });
    }

    [Test]
    public void CreateRasterizerState_Succeeds()
    {
        var descriptor = new RasterizerStateDescription
        {
            Mode = FillMode.Solid,
            Culling = FaceCulling.Back,
            Winding = FaceWinding.CounterClockwise,
            Scissor = true,
        };

        using var state = Device.CreateRasterizerState(descriptor);

        Assert.Multiple(() =>
        {
            Assert.That(state.Mode, Is.EqualTo(descriptor.Mode));
            Assert.That(state.Culling, Is.EqualTo(descriptor.Culling));
            Assert.That(state.Winding, Is.EqualTo(descriptor.Winding));
            Assert.That(state.Scissor, Is.EqualTo(descriptor.Scissor));
        });
    }

    [Test]
    public void CreateBlendState_Succeeds()
    {
        var descriptor = BlendStateDescription.NonPremultiplied;

        using var state = Device.CreateBlendState(descriptor);

        Assert.Multiple(() =>
        {
            Assert.That(state.Enabled, Is.EqualTo(descriptor.Enabled));
            Assert.That(state.SourceColor, Is.EqualTo(descriptor.SourceColor));
            Assert.That(state.SourceAlpha, Is.EqualTo(descriptor.SourceAlpha));
            Assert.That(state.DestinationColor, Is.EqualTo(descriptor.DestinationColor));
            Assert.That(state.DestinationAlpha, Is.EqualTo(descriptor.DestinationAlpha));
            Assert.That(state.ColorOperation, Is.EqualTo(descriptor.ColorOperation));
            Assert.That(state.AlphaOperation, Is.EqualTo(descriptor.AlphaOperation));
            Assert.That(state.ColorOperation, Is.EqualTo(descriptor.ColorOperation));
        });
    }
}
