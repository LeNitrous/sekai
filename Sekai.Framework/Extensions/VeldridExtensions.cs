// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Platform;

namespace Sekai.Framework.Extensions;

internal static class VeldridExtensions
{
    public static Veldrid.BufferUsage ToVeldrid(this BufferUsage usage)
    {
        return (Veldrid.BufferUsage)usage;
    }

    public static Veldrid.MapMode ToVeldrid(this MapMode mode)
    {
        if (mode.HasFlag(MapMode.Read) && mode.HasFlag(MapMode.Write))
            return Veldrid.MapMode.ReadWrite;

        if (mode == MapMode.Read)
            return Veldrid.MapMode.Read;

        if (mode == MapMode.Write)
            return Veldrid.MapMode.Write;

        throw new ArgumentOutOfRangeException(nameof(mode));
    }

    public static Veldrid.GraphicsBackend ToVeldrid(this GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => Veldrid.GraphicsBackend.Direct3D11,
            GraphicsAPI.Vulkan => Veldrid.GraphicsBackend.Vulkan,
            GraphicsAPI.OpenGL => RuntimeInfo.IsDesktop ? Veldrid.GraphicsBackend.OpenGL : Veldrid.GraphicsBackend.OpenGLES,
            GraphicsAPI.Metal => Veldrid.GraphicsBackend.Metal,
            _ => throw new NotSupportedException()
        };
    }
}
