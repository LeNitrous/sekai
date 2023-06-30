// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;

namespace Sekai.Graphics.Tests;

public abstract class GraphicsBufferTest<T> : GraphicsTest<T>
    where T : GraphicsDeviceCreator, new()
{
    [Test]
    public void GetSetData_TData_Succeeds()
    {
        int send = 69;
        int recv = 0;

        using var buffer = Device.CreateBuffer<int>(BufferType.Uniform, 1);
        buffer.SetData(send);
        buffer.GetData(ref recv);

        Assert.That(recv, Is.EqualTo(send));
    }

    [Test]
    public void GetSetData_Array_Succeeds()
    {
        int[] send = new int[] { 1, 2, 3, 4 };
        int[] recv = new int[4];

        using var buffer = Device.CreateBuffer<int>(BufferType.Uniform, 4);
        buffer.SetData((ReadOnlySpan<int>)send);
        buffer.GetData(recv);

        Assert.That(recv, Is.EqualTo(send));
    }

    [Test]
    public void GetSetData_Span_Succeeds()
    {
        Span<int> send = stackalloc int[] { 1, 2, 3, 4 };
        Span<int> recv = stackalloc int[4];

        using var buffer = Device.CreateBuffer<int>(BufferType.Uniform, 4);
        buffer.SetData((ReadOnlySpan<int>)send);
        buffer.GetData(recv);

        Assert.That(send.SequenceEqual(recv), Is.True);
    }

    [Test]
    public unsafe void Map_Succeeds()
    {
        const int expected = 69;

        using var buffer = Device.CreateBuffer<int>(BufferType.Uniform, 1);

        using (var m = buffer.Map<int>(MapMode.ReadWrite))
        {
            m[0] = expected;
        }

        int value = buffer.GetData<int>();

        Assert.That(value, Is.EqualTo(expected));
    }
}
