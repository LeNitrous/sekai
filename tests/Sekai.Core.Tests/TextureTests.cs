// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Graphics.Textures;

namespace Sekai.Core.Tests;

public class TextureTests : GraphicsDeviceTest
{
    [Test]
    public void Can_Create_Texture1D()
    {
        Texture texture = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => texture = Texture.Create(Graphics, 1), Throws.Nothing);
            Assert.That(texture.Kind, Is.EqualTo(TextureKind.Texture1D));
            Assert.That(texture.Width, Is.EqualTo(1));
            Assert.That(() => Texture.Create(Graphics, -1), Throws.ArgumentException);
        });
    }

    [Test]
    public void Can_Create_Texture2D()
    {
        Texture texture = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => texture = Texture.Create(Graphics, 1, 1), Throws.Nothing);
            Assert.That(texture.Kind, Is.EqualTo(TextureKind.Texture2D));
            Assert.That(texture.Width, Is.EqualTo(1));
            Assert.That(texture.Height, Is.EqualTo(1));
            Assert.That(() => Texture.Create(Graphics, -1, -1), Throws.ArgumentException);
        });
    }

    [Test]
    public void Can_Create_Texture3D()
    {
        Texture texture = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => texture = Texture.Create(Graphics, 1, 1, 1), Throws.Nothing);
            Assert.That(texture.Kind, Is.EqualTo(TextureKind.Texture3D));
            Assert.That(texture.Width, Is.EqualTo(1));
            Assert.That(texture.Height, Is.EqualTo(1));
            Assert.That(texture.Depth, Is.EqualTo(1));
            Assert.That(() => Texture.Create(Graphics, -1, -1, -1), Throws.ArgumentException);
        });
    }

    [Test]
    public void Can_ReadWrite_From_Array()
    {
        using var texture = Texture.Create(Graphics, 1);
        byte[] send = new byte[] { 255, 255, 255, 255 };
        byte[] recv = new byte[4];

        Assert.Multiple(() =>
        {
            Assert.That(() => texture.SetData(Graphics, send, 0, 0, 0, 1, 1, 1, 0, 0), Throws.Nothing);
            Assert.That(() => texture.GetData(Graphics, recv, 0, 0), Throws.Nothing);
            Assert.That(send, Is.EqualTo(recv));
        });
    }

    [Test]
    public void Can_ReadWrite_From_Value()
    {
        using var texture = Texture.Create(Graphics, 1);
        var send = new Rgba8 { R = 255, G = 255, B = 255, A = 255 };
        var recv = default(Rgba8);

        Assert.Multiple(() =>
        {
            Assert.That(() => texture.SetData(Graphics, send, 0, 0, 0, 1, 1, 1, 0, 0), Throws.Nothing);
            Assert.That(() => recv = texture.GetData<Rgba8>(Graphics, 0, 0), Throws.Nothing);
            Assert.That(recv.R, Is.EqualTo(255));
            Assert.That(recv.G, Is.EqualTo(255));
            Assert.That(recv.B, Is.EqualTo(255));
            Assert.That(recv.A, Is.EqualTo(255));
        });
    }

    private struct Rgba8
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
}
