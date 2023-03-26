// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Graphics;

namespace Sekai.Core.Tests;

public class BufferTests : GraphicsDeviceTest
{
    [Test]
    public void Create_Empty_Succeeds()
    {
        Buffer<short> buffer = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer = Buffer.Index.Create<short>(Graphics, 10, true), Throws.Nothing);
            Assert.That(buffer.Length, Is.EqualTo(10));
        });

        buffer.Dispose();
    }

    [Test]
    public void Create_Empty_Fails()
    {
        Assert.That(() => Buffer.Index.Create<short>(Graphics, -10, true), Throws.ArgumentException);
    }

    [Test]
    public void Create_From_Value()
    {
        Buffer<short> buffer = null!;
        const short send = 1337;

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer = Buffer.Index.Create(Graphics, send, true), Throws.Nothing);
            Assert.That(buffer.Length, Is.EqualTo(1));
            Assert.That(buffer.GetData(Graphics), Is.EqualTo(send));
        });

        buffer.Dispose();
    }

    [Test]
    public void Create_From_Value_Ref()
    {
        Buffer<short> buffer = null!;
        short send = 1337;

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer = Buffer.Index.Create(Graphics, ref send, 1, true), Throws.Nothing);
            Assert.That(buffer.Length, Is.EqualTo(1));

            short recv = 0;

            Assert.That(() => buffer.GetData(Graphics, ref recv), Throws.Nothing);
            Assert.That(recv, Is.EqualTo(send));
        });

        buffer.Dispose();
    }

    [Test]
    public void Create_From_Array()
    {
        Buffer<short> buffer = null!;
        short[] send = new short[] { 0, 1, 2, 3 };

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer = Buffer.Index.Create(Graphics, send, true), Throws.Nothing);
            Assert.That(buffer.Length, Is.EqualTo(4));

            short[] recv = new short[4];

            Assert.That(() => buffer.GetData(Graphics, recv), Throws.Nothing);
            Assert.That(recv, Is.EqualTo(send));
        });

        buffer.Dispose();
    }

    [Test]
    public void Create_From_Span()
    {
        System.Span<short> send = stackalloc short[] { 0, 1, 2, 3 };
        var buffer = Buffer.Index.Create<short>(Graphics, send, true);

        Assert.That(buffer.Length, Is.EqualTo(4));

        System.Span<short> recv = stackalloc short[4];
        buffer.GetData(Graphics, recv);

        Assert.That(System.MemoryExtensions.SequenceEqual(send, recv), Is.True);

        buffer.Dispose();
    }

    [Test]
    public void Update_From_Value()
    {
        var buffer = createBuffer(1);

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer.SetData(Graphics, 1337), Throws.Nothing);
            Assert.That(buffer.GetData(Graphics), Is.EqualTo(1337));
        });

        buffer.Dispose();
    }

    [Test]
    public void Update_From_Value_Ref()
    {
        var buffer = createBuffer(1);
        short send = 1337;

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer.SetData(Graphics, ref send), Throws.Nothing);

            short recv = 0;

            Assert.That(() => buffer.GetData(Graphics, ref recv), Throws.Nothing);
            Assert.That(recv, Is.EqualTo(send));
        });

        buffer.Dispose();
    }

    [Test]
    public void Update_From_Array()
    {
        var buffer = createBuffer(4);
        short[] send = new short[] { 0, 1, 2, 3 };

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer.SetData(Graphics, send), Throws.Nothing);

            short[] recv = new short[4];

            Assert.That(() => buffer.GetData(Graphics, recv), Throws.Nothing);
            Assert.That(recv, Is.EqualTo(send));
        });

        buffer.Dispose();
    }

    [Test]
    public void Update_From_Span()
    {
        var buffer = createBuffer(4);
        short[] send = new short[] { 0, 1, 2, 3 };

        Assert.Multiple(() =>
        {
            Assert.That(() => buffer.SetData(Graphics, send), Throws.Nothing);

            System.Span<short> recv = stackalloc short[4];
            buffer.GetData(Graphics, recv);

            Assert.That(System.MemoryExtensions.SequenceEqual(recv, send), Is.True);
        });

        buffer.Dispose();
    }

    [Test]
    public void Update_OutOfBounds()
    {
        var buffer = createBuffer(1);
        Assert.That(() => buffer.SetData(Graphics, new short[] { 1, 2, 3 }), Throws.ArgumentException);
    }

    private Buffer<short> createBuffer(int count)
    {
        return Buffer.Index.Create<short>(Graphics, count, true);
    }
}
