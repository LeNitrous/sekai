// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Platform;
using Silk.NET.Windowing;
using Veldrid;
using SilkGraphicsAPI = Silk.NET.Windowing.GraphicsAPI;

namespace Sekai.Framework.Graphics;

public enum GraphicsAPI
{
    Direct3D11,
    OpenGL,
    Vulkan,
    Metal,
}

internal static class GraphicsAPIExtensions
{
    public static GraphicsBackend ToVeldrid(this GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => GraphicsBackend.Direct3D11,
            GraphicsAPI.Vulkan => GraphicsBackend.Vulkan,
            GraphicsAPI.OpenGL => RuntimeInfo.IsDesktop ? GraphicsBackend.OpenGL : GraphicsBackend.OpenGLES,
            GraphicsAPI.Metal => GraphicsBackend.Metal,
            _ => throw new NotSupportedException()
        };
    }

    public static SilkGraphicsAPI ToSilk(this GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => SilkGraphicsAPI.None,
            GraphicsAPI.Vulkan => SilkGraphicsAPI.DefaultVulkan,
            GraphicsAPI.OpenGL => RuntimeInfo.IsDesktop ? new SilkGraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 5)) : new SilkGraphicsAPI(ContextAPI.OpenGLES, new APIVersion(3, 1)),
            GraphicsAPI.Metal => SilkGraphicsAPI.None,
            _ => throw new NotSupportedException(),
        };
    }
}
