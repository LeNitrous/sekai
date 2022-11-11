// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Textures;
using Sekai.OpenGL.Extensions;
using Silk.NET.OpenGL;
using SekaiPixelFmt = Sekai.Graphics.Textures.PixelFormat;

namespace Sekai.OpenGL;

internal unsafe class GLTexture : GLResource, INativeTexture
{
    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }
    public int Layers { get; }
    public int Levels { get; }
    public FilterMode Min { get; set; }
    public FilterMode Mag { get; set; }
    public WrapMode WrapModeS { get; set; }
    public WrapMode WrapModeT { get; set; }
    public WrapMode WrapModeR { get; set; }
    public SekaiPixelFmt Format { get; }
    public TextureType Type { get; }
    public TextureUsage Usage { get; }
    public TextureSampleCount SampleCount { get; }

    internal readonly TextureTarget Target;
    private readonly uint textureId;

    public GLTexture(GLGraphicsSystem context, int width, int height, int depth, int layers, int levels, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, SekaiPixelFmt format, TextureType type, TextureUsage usage, TextureSampleCount sampleCount)
        : base(context)
    {
        Min = min;
        Mag = mag;
        Type = type;
        Usage = usage;
        Width = width;
        Depth = depth;
        Height = height;
        Layers = layers;
        Levels = levels;
        Format = format;
        Target = getTextureTarget();
        WrapModeS = wrapModeS;
        WrapModeT = wrapModeT;
        WrapModeR = wrapModeR;
        textureId = GL.GenTexture();
        SampleCount = sampleCount;
        generateTexture();
    }

    public void SetData(nint data, int x, int y, int z, int width, int height, int depth, int level, int layer)
    {
        if (width == 0 || height == 0 || depth == 0)
            return;

        GL.BindTexture(Target, this);

        bool isCompressed = Format.IsCompressed();
        int blockLength = isCompressed ? 4 : 1;
        int blockAlignedWidth = Math.Max(width, blockLength);
        int blockAlignedHeight = Math.Max(height, blockLength);

        int rowPitch = Format.GetRowPitch(blockAlignedWidth);
        int depthPitch = Format.GetDepthPitch(blockAlignedWidth, blockAlignedHeight);

        getDimensionsAtLevel(level, out int w, out int h, out int d);
        width = Math.Min(width, w);
        height = Math.Min(width, h);

        int unpackAlignment = isCompressed ? 4 : Format.SizeOfFormat();

        if (unpackAlignment < 4)
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, unpackAlignment);

        switch (Target)
        {
            case TextureTarget.Texture1D when isCompressed:
                GL.CompressedTexSubImage1D(Target, level, x, (uint)width, Format.ToInternalFormat(), (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1D:
                GL.TexSubImage1D(Target, level, x, (uint)width, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture1DArray when isCompressed:
                GL.CompressedTexSubImage2D(Target, level, x, layer, (uint)width, 1, Format.ToInternalFormat(), (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1DArray:
                GL.TexSubImage2D(Target, level, x, layer, (uint)width, 1, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture2D when isCompressed:
                GL.CompressedTexSubImage2D(Target, level, x, y, (uint)width, (uint)height, Format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.Texture2D:
                GL.TexSubImage2D(Target, level, x, y, (uint)width, (uint)height, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture2DArray when isCompressed:
                GL.CompressedTexSubImage3D(Target, level, x, y, layer, (uint)width, (uint)height, 1, Format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.Texture2DArray:
                GL.TexSubImage3D(Target, level, x, y, layer, (uint)width, (uint)height, 1, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture3D when isCompressed:
                GL.CompressedTexSubImage3D(Target, level, x, y, z, (uint)width, (uint)height, (uint)depth, Format.ToInternalFormat(), (uint)(depthPitch * depth), (void*)data);
                break;

            case TextureTarget.Texture3D:
                GL.TexSubImage3D(Target, level, x, y, z, (uint)width, (uint)height, (uint)depth, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.TextureCubeMap when isCompressed:
                GL.CompressedTexSubImage2D(getCubemapTarget(layer), level, x, y, (uint)width, (uint)height, Format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMap:
                GL.TexSubImage2D(getCubemapTarget(layer), level, x, y, (uint)width, (uint)height, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray when isCompressed:
                GL.CompressedTexSubImage3D(Target, level, x, y, layer, (uint)width, (uint)height, 1, Format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray:
                GL.TexSubImage3D(Target, level, x, y, layer, (uint)width, (uint)height, 1, Format.ToPixelFormat(), Format.ToPixelType(), (void*)data);
                break;

            default:
                throw new NotSupportedException();
        }

        if (unpackAlignment < 4)
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
    }

    private void generateTexture()
    {
        GL.BindTexture(Target, textureId);
        GL.TexParameterI(Target, TextureParameterName.GenerateMipmapSgis, (int)GLEnum.False);
        GL.TexParameterI(Target, TextureParameterName.TextureMinFilter, Min.ToGLEnumInt());
        GL.TexParameterI(Target, TextureParameterName.TextureMagFilter, Mag.ToGLEnumInt());
        GL.TexParameterI(Target, TextureParameterName.TextureWrapS, WrapModeS.ToGLEnumInt());
        GL.TexParameterI(Target, TextureParameterName.TextureWrapT, WrapModeT.ToGLEnumInt());
        GL.TexParameterI(Target, TextureParameterName.TextureWrapR, WrapModeR.ToGLEnumInt());

        switch (Target)
        {
            case TextureTarget.Texture1D:
                generateTexture1D();
                break;

            case TextureTarget.Texture1DArray:
            case TextureTarget.Texture2D:
                generateTexture2D();
                break;

            case TextureTarget.Texture2DArray:
            case TextureTarget.Texture3D:
                generateTexture3D();
                break;

            case TextureTarget.Texture2DMultisample:
                generateTexture2DMultisample();
                break;

            case TextureTarget.Texture2DMultisampleArray:
                generateTexture2DMultisampleArray();
                break;

            case TextureTarget.TextureCubeMap:
                generateTextureCubemap();
                break;

            case TextureTarget.TextureCubeMapArray:
                generateTextureCubemapArray();
                break;
        }
    }

    private void generateTexture1D()
    {
        uint w = (uint)Width;
        for (int i = 0; i < Levels; i++)
        {
            GL.TexImage1D(TextureTarget.Texture1D, i, Format.ToInternalFormat(), w, 0, Format.ToPixelFormat(), Format.ToPixelType(), null);
            w = Math.Max(1, w / 2);
        }
    }

    private void generateTexture2D()
    {
        uint w = (uint)Width;
        uint h = Type == TextureType.Texture2D ? (uint)Height : (uint)Layers;
        for (int i = 0; i < Levels; i++)
        {
            GL.TexImage2D(Target, i, Format.ToInternalFormat(), w, h, 0, Format.ToPixelFormat(), Format.ToPixelType(), null);
            w = Math.Max(1, w / 2);

            if (Type == TextureType.Texture2D)
                h = Math.Max(1, h / 2);
        }
    }

    private void generateTexture3D()
    {
        uint w = (uint)Width;
        uint h = (uint)Height;
        uint d = Type == TextureType.Texture3D ? (uint)Depth : (uint)Layers;
        for (int i = 0; i < Levels; i++)
        {
            GL.TexImage3D(Target, i, Format.ToInternalFormat(), w, h, d, 0, Format.ToPixelFormat(), Format.ToPixelType(), null);
            w = Math.Max(1, w / 2);
            h = Math.Max(1, h / 2);

            if (Type == TextureType.Texture3D)
                d = Math.Max(1, d / 2);
        }
    }

    private void generateTexture2DMultisample()
    {
        GL.TexImage2DMultisample(TextureTarget.Texture2DMultisample, getSampleCount(), Format.ToInternalFormat(), (uint)Width, (uint)Height, false);
    }

    private void generateTexture2DMultisampleArray()
    {
        GL.TexImage3DMultisample(TextureTarget.Texture2DMultisampleArray, getSampleCount(), Format.ToInternalFormat(), (uint)Width, (uint)Height, (uint)Layers, false);
    }

    private void generateTextureCubemap()
    {
        uint w = (uint)Width;
        uint h = (uint)Height;
        for (int i = 0; i < Levels; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + j, i, Format.ToInternalFormat(), w, h, 0, Format.ToPixelFormat(), Format.ToPixelType(), null);
            }

            w = Math.Max(1, w / 2);
            h = Math.Max(1, h / 2);
        }
    }

    private void generateTextureCubemapArray()
    {
        uint w = (uint)Width;
        uint h = (uint)Height;
        for (int i = 0; i < Levels; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GL.TexImage3D(TextureTarget.TextureCubeMapPositiveX + j, i, Format.ToInternalFormat(), w, h, (uint)(Layers * 6), 0, Format.ToPixelFormat(), Format.ToPixelType(), null);
            }

            w = Math.Max(1, w / 2);
            h = Math.Max(1, h / 2);
        }
    }

    private uint getSampleCount()
    {
        switch (SampleCount)
        {
            default:
                return 1;

            case TextureSampleCount.Count2:
                return 2;

            case TextureSampleCount.Count4:
                return 4;

            case TextureSampleCount.Count8:
                return 8;

            case TextureSampleCount.Count16:
                return 16;

            case TextureSampleCount.Count32:
                return 32;
        }
    }

    private TextureTarget getTextureTarget()
    {
        if (Usage == TextureUsage.Cubemap && Layers > 1)
            return TextureTarget.TextureCubeMapArray;

        if (Usage == TextureUsage.Cubemap && Layers == 1)
            return TextureTarget.TextureCubeMap;

        if (Type == TextureType.Texture1D && Layers > 1)
            return TextureTarget.Texture1DArray;

        if (Type == TextureType.Texture1D && Layers == 1)
            return TextureTarget.Texture1D;

        if (Type == TextureType.Texture2D && SampleCount > TextureSampleCount.Count1 && Layers > 1)
            return TextureTarget.Texture2DMultisampleArray;

        if (Type == TextureType.Texture2D && SampleCount == TextureSampleCount.Count1 && Layers > 1)
            return TextureTarget.Texture2DArray;

        if (Type == TextureType.Texture2D && SampleCount > TextureSampleCount.Count1 && Layers == 1)
            return TextureTarget.Texture2DMultisample;

        if (Type == TextureType.Texture2D && SampleCount == TextureSampleCount.Count1 && Layers == 1)
            return TextureTarget.Texture2D;

        if (Type == TextureType.Texture3D)
            return TextureTarget.Texture3D;

        throw new NotSupportedException($@"Texture type ""{Type}"" is not supported.");
    }

    private void getDimensionsAtLevel(int level, out int width, out int height, out int depth)
    {
        width = getDimension(Width, level);
        depth = getDimension(Depth, level);
        height = getDimension(Height, level);
    }

    private static TextureTarget getCubemapTarget(int layer)
    {
        switch (layer)
        {
            case 0:
                return TextureTarget.TextureCubeMapPositiveX;

            case 1:
                return TextureTarget.TextureCubeMapNegativeX;

            case 2:
                return TextureTarget.TextureCubeMapPositiveY;

            case 3:
                return TextureTarget.TextureCubeMapNegativeY;

            case 4:
                return TextureTarget.TextureCubeMapPositiveZ;

            case 5:
                return TextureTarget.TextureCubeMapNegativeZ;

            default:
                throw new InvalidOperationException(@"Unexpected layer for a cubemap texture.");
        }
    }

    private static int getDimension(int dimension, int level)
    {
        for (int i = 0; i < level; i++)
        {
            dimension /= 2;
        }

        return Math.Max(1, dimension);
    }

    public static implicit operator uint(GLTexture texture) => texture.textureId;
}
