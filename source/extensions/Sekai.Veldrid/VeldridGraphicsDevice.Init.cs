// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Sekai.Framework.Windowing;
using Sekai.Framework.Windowing.OpenGL;
using SharpGen.Runtime;
using Veldrid.OpenGLBinding;
using Vortice.DXGI;
using Vulkan;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal partial class VeldridGraphicsDevice
{
    private void initializeDirect3D11(Vd.GraphicsDeviceOptions options, Vd.SwapchainDescription swapChainDescription)
    {
        GraphicsAPI = Framework.Graphics.GraphicsAPI.Direct3D11;
        device = Vd.GraphicsDevice.CreateD3D11(options, swapChainDescription);

        var info = device.GetD3D11Info();
        var adapter = MarshallingHelpers.FromPointer<IDXGIAdapter>(info.Adapter);
        Name = adapter?.Description.Description ?? string.Empty;
    }

    private unsafe void initializeOpenGL(IView view, Vd.GraphicsDeviceOptions options, Vd.SwapchainDescription swapChainDescription)
    {
        if (view is not IOpenGLProviderSource glSource)
            throw new InvalidOperationException(@"The view must be able to provide an OpenGL context.");

        var gl = glSource.GL;

        var openGLPlatformOptions = new Vd.OpenGL.OpenGLPlatformInfo
        (
            gl.Handle,
            gl.GetProcAddress,
            gl.MakeCurrent,
            gl.GetCurrentContext,
            gl.ClearCurrentContext,
            gl.DeleteContext,
            gl.SwapBuffers,
            gl.SetSyncToVerticalBlank
        );

        GraphicsAPI = Framework.Graphics.GraphicsAPI.OpenGL;
        device = Vd.GraphicsDevice.CreateOpenGL(options, openGLPlatformOptions, (uint)view.Size.Width, (uint)view.Size.Height);
        device.GetOpenGLInfo().ExecuteOnGLThread(() =>
        {
            Name = Marshal.PtrToStringUTF8((IntPtr)OpenGLNative.glGetString(StringName.Renderer)) ?? string.Empty;
        });
    }

    private unsafe void initializeOpenGLES(Vd.GraphicsDeviceOptions options, Vd.SwapchainDescription swapChainDescription)
    {
        GraphicsAPI = Framework.Graphics.GraphicsAPI.OpenGLES;
        device = Vd.GraphicsDevice.CreateOpenGLES(options, swapChainDescription);
        device.GetOpenGLInfo().ExecuteOnGLThread(() =>
        {
            Name = Marshal.PtrToStringUTF8((IntPtr)OpenGLNative.glGetString(StringName.Renderer)) ?? string.Empty;
        });
    }

    private unsafe void initializeVulkan(Vd.GraphicsDeviceOptions options, Vd.SwapchainDescription swapChainDescription)
    {
        GraphicsAPI = Framework.Graphics.GraphicsAPI.Vulkan;
        device = Vd.GraphicsDevice.CreateVulkan(options, swapChainDescription);

        var info = device.GetVulkanInfo();
        var physical = info.PhysicalDevice;

        VkPhysicalDeviceProperties props;
        VulkanNative.vkGetPhysicalDeviceProperties(physical, &props);
        Name = Marshal.PtrToStringUTF8((IntPtr)props.deviceName) ?? string.Empty;
    }

    private unsafe void initializeMetal(Vd.GraphicsDeviceOptions options, Vd.SwapchainDescription swapChainDescription)
    {
        GraphicsAPI = Framework.Graphics.GraphicsAPI.Metal;
        Name = "Metal";
        device = Vd.GraphicsDevice.CreateMetal(options, swapChainDescription);
    }
}
