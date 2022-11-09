// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Sekai.Graphics.Textures;

/// <summary>
/// A storage object containing color data which can be used to draw on the screen.
/// </summary>
public class Texture : FrameworkObject
{
    /// <inheritdoc cref="INativeTexture.Width"/>
    public int Width => Native.Width;

    /// <inheritdoc cref="INativeTexture.Height"/>
    public int Height => Native.Height;

    /// <inheritdoc cref="INativeTexture.Layers"/>
    public int Layers => Native.Layers;

    /// <inheritdoc cref="INativeTexture.Levels"/>
    public int Level => Native.Levels;

    /// <inheritdoc cref="INativeTexture.Min"/>
    public FilterMode Min
    {
        get => Native.Min;
        set => Native.Min = value;
    }

    /// <inheritdoc cref="INativeTexture.Mag"/>
    public FilterMode Mag
    {
        get => Native.Mag;
        set => Native.Mag = value;
    }

    /// <inheritdoc cref="INativeTexture.WrapModeS"/>
    public WrapMode WrapModeS
    {
        get => Native.WrapModeS;
        set => Native.WrapModeS = value;
    }

    /// <inheritdoc cref="INativeTexture.WrapModeT"/>
    public WrapMode WrapModeT
    {
        get => Native.WrapModeT;
        set => Native.WrapModeT = value;
    }

    /// <summary>
    /// Gets whether this texture is currently bound.
    /// </summary>
    public bool IsBound { get; private set; }

    internal readonly INativeTexture Native;
    private readonly GraphicsContext context = Game.Resolve<GraphicsContext>();
    private readonly IGraphicsFactory factory = Game.Resolve<IGraphicsFactory>();

    public Texture()
    {
        Native = factory.CreateTexture();
    }

    /// <inheritdoc cref="INativeTexture.Bind"/>
    public void Bind(int unit = 0)
    {
        context.BindTexture(this, unit);
    }

    public void SetData(nint data, int size, int layer = 0, int level = 0, int offset = 0)
    {
        Native.SetData(data, size, layer, level, offset);
    }

    public void SetData<T>(T data, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        SetData(ref data, layer, level, offset);
    }

    public unsafe void SetData<T>(ref T data, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        fixed (T* ptr = &data)
            SetData((nint)ptr, Marshal.SizeOf<T>(), layer, level, offset);
    }

    public unsafe void SetData<T>(ReadOnlySpan<T> data, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        fixed (T* ptr = data)
            SetData((nint)ptr, Marshal.SizeOf<T>() * data.Length, layer, level, offset);
    }

    public void SetData<T>(T[] data, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        SetData<T>(data.AsSpan(), layer, level, offset);
    }

    public void GetData(nint dest, int size, int layer = 0, int level = 0, int offset = 0)
    {
        Native.GetData(dest, size, layer, level, offset);
    }

    public T GetData<T>(int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        T dest = default;
        GetData(ref dest, layer, level, offset);
        return dest;
    }

    public unsafe void GetData<T>(ref T dest, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        fixed (T* ptr = &dest)
            GetData((nint)ptr, Marshal.SizeOf<T>(), layer, level, offset);
    }

    public unsafe void GetData<T>(ReadOnlySpan<T> dest, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        fixed (T* ptr = dest)
            GetData((nint)ptr, Marshal.SizeOf<T>() * dest.Length, layer, level, offset);
    }

    public void GetData<T>(T[] data, int layer = 0, int level = 0, int offset = 0)
        where T : unmanaged
    {
        GetData<T>(data.AsSpan(), layer, level, offset);
    }

    protected override void Destroy()
    {
        Native.Dispose();
    }
}
