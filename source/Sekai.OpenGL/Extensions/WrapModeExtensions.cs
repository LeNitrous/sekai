// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Textures;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL.Extensions;

internal static class WrapModeExtensions
{
    public static int ToGLEnumInt(this WrapMode mode)
    {
        return mode switch
        {
            WrapMode.None or WrapMode.ClampToBorder => (int)GLEnum.ClampToBorder,
            WrapMode.ClampToEdge => (int)GLEnum.ClampToEdge,
            WrapMode.Repeat => (int)GLEnum.Repeat,
            WrapMode.RepeatMirrored => (int)GLEnum.MirroredRepeat,
            _ => throw new NotSupportedException($@"Wrap mode ""{mode}"" is not supported."),
        };
    }
}
