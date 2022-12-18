// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Textures;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL.Extensions;

internal static class FilterModeExtensions
{
    public static int ToGLEnumInt(this FilterMode mode)
    {
        return mode switch
        {
            FilterMode.Linear => (int)GLEnum.Linear,
            FilterMode.Nearest => (int)GLEnum.Nearest,
            _ => throw new NotSupportedException($@"Filter mode ""{mode}"" is not supported."),
        };
    }
}
