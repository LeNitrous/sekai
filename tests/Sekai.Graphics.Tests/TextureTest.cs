// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;

namespace Sekai.Graphics.Tests;

public abstract class TextureTest<T> : GraphicsTest<T>
    where T : GraphicsDeviceCreator, new()
{
    [Test]
    public void GetSetData_Array_Succeeds()
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

        byte[] send = new byte[] { 127, 127, 127, 255 };
        byte[] recv = new byte[4];

        texture.SetData(send, 0, 0, 0, 0, 0, 1, 1, 1);
        texture.GetData(recv, 0, 0, 0, 0, 0, 1, 1, 1);

        Assert.That(recv, Is.EqualTo(send));
    }

    [Test]
    public void GetSetData_Span_Succeeds()
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

        Span<byte> send = stackalloc byte[] { 127, 127, 127, 255 };
        Span<byte> recv = stackalloc byte[4];

        texture.SetData((ReadOnlySpan<byte>)send, 0, 0, 0, 0, 0, 1, 1, 1);
        texture.GetData(recv, 0, 0, 0, 0, 0, 1, 1, 1);

        Assert.That(send.SequenceEqual(recv), Is.True);
    }
}
