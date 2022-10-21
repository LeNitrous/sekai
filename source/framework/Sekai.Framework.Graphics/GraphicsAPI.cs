// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// The graphics API to be used.
/// </summary>
public enum GraphicsAPI
{
    /// <summary>
    /// Direct3D 11.
    /// </summary>
    Direct3D11,

    /// <summary>
    /// Vulkan.
    /// </summary>
    Vulkan,

    /// <summary>
    /// OpenGL.
    /// </summary>
    OpenGL,

    /// <summary>
    /// Metal.
    /// </summary>
    Metal,

    /// <summary>
    /// OpenGL ES.
    /// </summary>
    OpenGLES,
}

public static class RuntimeInfoPlatformExtensions
{
    public static GraphicsAPI GetGraphicsAPI(this RuntimeInfo.Platform platform)
    {
        return platform switch
        {
            RuntimeInfo.Platform.Windows => GraphicsAPI.Direct3D11,
            RuntimeInfo.Platform.Android or RuntimeInfo.Platform.Linux => GraphicsAPI.Vulkan,
            RuntimeInfo.Platform.macOS or RuntimeInfo.Platform.iOS => GraphicsAPI.Metal,
            _ => RuntimeInfo.IsMobile ? GraphicsAPI.OpenGLES : GraphicsAPI.OpenGL,
        };
    }
}
