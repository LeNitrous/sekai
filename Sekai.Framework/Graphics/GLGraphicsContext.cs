// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.InteropServices;
using System.Text;
using Sekai.Framework.Logging;
using Sekai.Framework.Platform;
using Veldrid;
using Veldrid.OpenGLBinding;

namespace Sekai.Framework.Graphics;

internal sealed class GLGraphicsContext : GraphicsContext
{
    protected override GraphicsBackend Backend => RuntimeInfo.IsDesktop ? GraphicsBackend.OpenGL : GraphicsBackend.OpenGLES;

    protected override unsafe void Initialize()
    {
        string vendor = string.Empty;
        string version = string.Empty;
        string renderer = string.Empty;
        string shaderVersion = string.Empty;
        StringBuilder extensions = new();

        Device.GetOpenGLInfo().ExecuteOnGLThread(() =>
        {
            vendor = Marshal.PtrToStringUTF8((nint)OpenGLNative.glGetString(StringName.Vendor)) ?? string.Empty;
            version = Marshal.PtrToStringUTF8((nint)OpenGLNative.glGetString(StringName.Version)) ?? string.Empty;
            renderer = Marshal.PtrToStringUTF8((nint)OpenGLNative.glGetString(StringName.Renderer)) ?? string.Empty;
            shaderVersion = Marshal.PtrToStringUTF8((nint)OpenGLNative.glGetString((StringName)35724)) ?? string.Empty;

            int extensionCount;
            OpenGLNative.glGetIntegerv(GetPName.NumExtensions, &extensionCount);

            for (uint i = 0; i < extensionCount; i++)
            {
                if (i > 0)
                    extensions.Append(' ');

                extensions.Append(Marshal.PtrToStringUTF8((nint)OpenGLNative.glGetStringi(StringNameIndexed.Extensions, i)));
            }
        });

        string gl = RuntimeInfo.IsDesktop ? "GL" : "GL ES";

        Logger.Log($@"{gl} Initialized");
        Logger.Log($@"{gl} Version:                 {version}");
        Logger.Log($@"{gl} Renderer:                {renderer}");
        Logger.Log($@"{gl} Shader Language Version: {shaderVersion}");
        Logger.Log($@"{gl} Vendor:                  {vendor}");
        Logger.Log($@"{gl} Extensions:              {extensions}");
    }
}
