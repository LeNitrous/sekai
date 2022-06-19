// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Platform;
using Silk.NET.Windowing;
using SilkGraphicsAPI = Silk.NET.Windowing.GraphicsAPI;

namespace Sekai.Framework.Extensions;

internal static class SilkExtensions
{
    public static SilkGraphicsAPI ToSilk(this Graphics.GraphicsAPI api)
    {
        return api switch
        {
            Graphics.GraphicsAPI.Direct3D11 => SilkGraphicsAPI.None,
            Graphics.GraphicsAPI.Vulkan => SilkGraphicsAPI.DefaultVulkan,
            Graphics.GraphicsAPI.OpenGL => RuntimeInfo.IsDesktop ? new SilkGraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 5)) : new SilkGraphicsAPI(ContextAPI.OpenGLES, new APIVersion(3, 1)),
            Graphics.GraphicsAPI.Metal => SilkGraphicsAPI.None,
            _ => throw new NotSupportedException(),
        };
    }
}
