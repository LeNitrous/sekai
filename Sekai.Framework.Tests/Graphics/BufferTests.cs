// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Framework.Graphics.Buffers;

namespace Sekai.Framework.Tests.Graphics;

public class BufferTests : FrameworkTestScene
{
    [Test]
    public void TestBufferGeneric()
    {
        const int length = 6;
        using var buffer = new Buffer<int>(length, BufferUsage.Index);
        Assert.Multiple(() =>
        {
            Assert.That(buffer.Size, Is.EqualTo(sizeof(int) * length));
            Assert.That(() => buffer.SetData(1), Throws.Nothing);
            Assert.That(() => buffer.SetData(new[] { 1, 2, 3, 4, 5, 6 }), Throws.Nothing);
            Assert.That(() => buffer.SetData(new[] { 1, 2, 3, 4, 5, 6, 7 }), Throws.InstanceOf<System.ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void TestBufferGetData()
    {
        using var a = new Buffer<int>(1, BufferUsage.Index);
        a.SetData(256);

        int num = 0;
        a.GetData(ref num);

        Assert.That(num, Is.EqualTo(256));
    }

    [Test]
    public void TestBufferGetDataArray()
    {
        int[] src = new[] { 1, 2, 3 };

        using var a = new Buffer<int>(src.Length, BufferUsage.Index);
        a.SetData(src);

        int[] dst = new int[3];
        a.GetData(dst);

        Assert.That(src, Is.EquivalentTo(dst));
    }
}
