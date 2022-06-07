// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Sekai.Framework.Logging;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;
using Vulkan;

namespace Sekai.Framework.Graphics;

public class GraphicsContext : FrameworkComponent, IGraphicsContext
{
    public GraphicsDevice Device { get; private set; }

    public GraphicsContext(IView view, GraphicsBackend backend)
    {
        Device = view.CreateGraphicsDevice
        (
            new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            }, backend
        );

        switch (backend)
        {
            case GraphicsBackend.Vulkan:
                initializeVulkan();
                break;

            case GraphicsBackend.Direct3D11:
            case GraphicsBackend.OpenGL:
            case GraphicsBackend.Metal:
            case GraphicsBackend.OpenGLES:
            default:
                throw new NotSupportedException();
        }
    }

    private unsafe void initializeVulkan()
    {
        if (!Device.GetVulkanInfo(out var info))
            return;

        var vkDevice = info.PhysicalDevice;

        VkPhysicalDeviceProperties properties;
        VulkanNative.vkGetPhysicalDeviceProperties(vkDevice, &properties);

        string deviceName = Marshal.PtrToStringUTF8((IntPtr)properties.deviceName) ?? string.Empty;
        string apiVersion = $"{properties.apiVersion >> 22}.{(properties.apiVersion >> 12) & 0x3FFU}.{properties.apiVersion & 0xFFFU}";
        string driverVersion = properties.vendorID switch
        {
            // NVIDIA
            0x10DE => $"{properties.driverVersion >> 22}.{(properties.driverVersion >> 14) & 0x0FFU}.{(properties.driverVersion >> 6) & 0x0FFU}.{properties.driverVersion & 0x003U}",

            // Intel
            0x8086 => $"{properties.driverVersion >> 22}.{properties.driverVersion & 0x3FFFU}",

            // Vulkan
            _ => $"{properties.driverVersion >> 22}.{(properties.driverVersion >> 12) & 0x3FFU}.{properties.driverVersion & 0xFFFU}"
        };

        Logger.Log($@"Vulkan Initialized");
        Logger.Log($@"Vulkan Device: {deviceName}");
        Logger.Log($@"Vulkan API Version: {apiVersion}");
        Logger.Log($@"Vulkan Driver Version: {driverVersion}");
    }
}
