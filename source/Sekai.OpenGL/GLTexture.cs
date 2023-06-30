// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed unsafe class GLTexture : Graphics.Texture
{
    public override TextureType Type { get; }
    public override int Width { get; }
    public override int Height { get; }
    public override int Depth { get; }
    public override Graphics.PixelFormat Format { get; }
    public override int Levels { get; }
    public override int Layers { get; }
    public override TextureUsage Usage { get; }
    public override TextureSampleCount Count { get; }

    private bool isDisposed;
    private readonly uint handle;
    private readonly bool isCompressed;
    private readonly bool isRenderbuffer;
    private readonly PixelType type;
    private readonly TextureTarget target;
    private readonly InternalFormat iFormat;
    private readonly Silk.NET.OpenGL.PixelFormat pFormat;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLTexture(GL gl, TextureDescription description)
    {
        GL = gl;
        Type = description.Type;
        Width = description.Width;
        Height = description.Height;
        Depth = description.Depth;
        Levels = description.Levels == 0 ? calculateMipLevels(description.Width, description.Height, description.Depth) : description.Levels;
        Layers = description.Layers;
        Format = description.Format;
        Count = description.Count;
        Usage = description.Usage;
        pFormat = description.Format.AsPixelFormat();
        iFormat = description.Format.AsInternalFormat();
        type = description.Format.AsPixelType();
        isCompressed = description.Format.IsCompressed();
        isRenderbuffer = (description.Usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget && (description.Usage & TextureUsage.Resource) != TextureUsage.Resource;

        if (isRenderbuffer)
        {
            handle = GL.GenRenderbuffer();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, iFormat, (uint)description.Width, (uint)description.Height);
        }
        else
        {
            handle = GL.GenTexture();
            target = description.AsTarget();

            GL.BindTexture(target, handle);

            switch (target)
            {
                case TextureTarget.Texture1D:
                    {
                        uint w = (uint)Width;

                        for (int i = 0; i < Levels; i++)
                        {
                            GL.TexImage1D(target, i, iFormat, w, 0, pFormat, type, null);
                            w = Math.Max(1, w / 2);
                        }

                        break;
                    }

                case TextureTarget.Texture1DArray:
                case TextureTarget.Texture2D:
                    {
                        uint w = (uint)Width;
                        uint h = target == TextureTarget.Texture2D ? (uint)Height : (uint)Layers;

                        for (int i = 0; i < Levels; i++)
                        {
                            GL.TexImage2D(target, i, iFormat, w, h, 0, pFormat, type, null);
                            w = Math.Max(1, w / 2);

                            if (target == TextureTarget.Texture2D)
                            {
                                h = Math.Max(1, h / 2);
                            }
                        }

                        break;
                    }

                case TextureTarget.Texture2DArray:
                case TextureTarget.Texture3D:
                    {
                        uint w = (uint)Width;
                        uint h = (uint)Height;
                        uint d = target == TextureTarget.Texture3D ? (uint)Depth : (uint)Layers;

                        for (int i = 0; i < Levels; i++)
                        {
                            GL.TexImage3D(target, i, iFormat, w, h, d, 0, pFormat, type, null);
                            w = Math.Max(1, w / 2);
                            h = Math.Max(1, h / 2);

                            if (target == TextureTarget.Texture3D)
                            {
                                d = Math.Max(1, d / 2);
                            }
                        }

                        break;
                    }

                case TextureTarget.Texture2DMultisample:
                    GL.TexImage2DMultisample(target, Count.GetSampleCount(), iFormat, (uint)Width, (uint)Height, false);
                    break;

                case TextureTarget.Texture2DMultisampleArray:
                    GL.TexImage3DMultisample(target, Count.GetSampleCount(), iFormat, (uint)Width, (uint)Height, (uint)Depth, false);
                    break;

                case TextureTarget.TextureCubeMap:
                    {
                        uint w = (uint)Width;
                        uint h = (uint)Height;
                        for (int i = 0; i < Levels; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + j, i, iFormat, w, h, 0, pFormat, type, null);
                            }

                            w = Math.Max(1, w / 2);
                            h = Math.Max(1, h / 2);
                        }
                        break;
                    }

                case TextureTarget.TextureCubeMapArray:
                    {
                        uint w = (uint)Width;
                        uint h = (uint)Height;
                        for (int i = 0; i < Levels; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                GL.TexImage3D(TextureTarget.TextureCubeMapPositiveX + j, i, iFormat, w, h, (uint)(Layers * 6), 0, pFormat, type, null);
                            }

                            w = Math.Max(1, w / 2);
                            h = Math.Max(1, h / 2);
                        }
                        break;
                    }
            }
        }
    }

    public void Bind(uint slot)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLTexture));
        }

        GL.ActiveTexture(TextureUnit.Texture0 + (int)slot);
        GL.BindTexture(target, handle);
    }

    public void Attach(int slot, int level, int layer)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLTexture));
        }

        var attachment = Format.IsDepthStencil()
            ? Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment
            : Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0 + slot;

        if (isRenderbuffer)
        {
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, handle);
        }
        else
        {
            if (Layers > 1)
            {
                GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, attachment, handle, level, layer);
            }
            else
            {
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, target, handle, level);
            }
        }
    }

    public override void SetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        int blockLength = isCompressed ? 4 : 1;
        int blockAlignedWidth = Math.Max(width, blockLength);
        int blockAlignedHeight = Math.Max(height, blockLength);

        int rowPitch = Format.GetRowPitch(blockAlignedWidth);
        int depthPitch = Format.GetDepthPitch(blockAlignedWidth, blockAlignedHeight);

        getDimensionsAtLevel(Width, Height, Depth, level, out int w, out int h, out int d);
        width = Math.Min(width, w);
        height = Math.Min(height, h);

        int unpackAlignment = isCompressed ? 4 : Format.SizeOfFormat();

        GL.BindTexture(target, handle);

        if (unpackAlignment < 4)
        {
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, unpackAlignment);
        }

        switch (target)
        {
            case TextureTarget.Texture1D when isCompressed:
                GL.CompressedTexSubImage1D(target, level, x, (uint)width, iFormat, (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1D:
                GL.TexSubImage1D(target, level, x, (uint)width, pFormat, type, (void*)data);
                break;

            case TextureTarget.Texture1DArray when isCompressed:
                GL.CompressedTexSubImage2D(target, level, x, layer, (uint)width, 1, iFormat, (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1DArray:
                GL.TexSubImage2D(target, level, x, layer, (uint)width, 1, pFormat, type, (void*)data);
                break;

            case TextureTarget.Texture2D when isCompressed:
                GL.CompressedTexSubImage2D(target, level, x, y, (uint)width, (uint)height, iFormat, (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture2D:
                GL.TexSubImage2D(target, level, x, y, (uint)width, (uint)height, pFormat, type, (void*)data);
                break;

            case TextureTarget.Texture2DArray when isCompressed:
                GL.CompressedTexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, iFormat, (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.Texture2DArray:
                GL.TexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, pFormat, type, (void*)data);
                break;

            case TextureTarget.Texture3D:
                GL.TexSubImage3D(target, level, x, y, z, (uint)width, (uint)height, (uint)depth, pFormat, type, (void*)data);
                break;

            case TextureTarget.TextureCubeMap when isCompressed:
                GL.CompressedTexSubImage2D(TextureTarget.TextureCubeMapPositiveX + layer, level, x, y, (uint)width, (uint)height, iFormat, (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMap:
                GL.TexSubImage2D(TextureTarget.TextureCubeMapPositiveX + layer, level, x, y, (uint)width, (uint)height, pFormat, type, (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray when isCompressed:
                GL.CompressedTexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, iFormat, (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray:
                GL.TexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, pFormat, type, (void*)data);
                break;
        }

        if (unpackAlignment < 4)
        {
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
        }
    }

    public override void GetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        GL.GetTextureSubImage(handle, level, x, y, z, (uint)width, (uint)height, (uint)depth, pFormat, type, size, (void*)data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int calculateMipLevels(int width, int height, int depth)
    {
        return (int)Math.Floor(Math.Log2(Math.Max(width, Math.Max(height, depth))));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void getDimensionsAtLevel(int inWidth, int inHeight, int inDepth, int level, out int outWidth, out int outHeight, out int outDepth)
    {
        outWidth = getTextureDimension(inWidth, level);
        outDepth = getTextureDimension(inDepth, level);
        outHeight = getTextureDimension(inHeight, level);

        static int getTextureDimension(int dimension, int level)
        {
            for (int i = 0; i < level; i++)
            {
                dimension /= 2;
            }

            return Math.Max(1, dimension);
        }
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        if (isRenderbuffer)
        {
            GL.DeleteRenderbuffer(handle);
        }
        else
        {
            GL.DeleteTexture(handle);
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
