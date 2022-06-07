// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sekai.Framework.Logging;
using SharpDX;
using SharpDX.DXGI;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;
using Veldrid.OpenGLBinding;
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

            case GraphicsBackend.OpenGL:
            case GraphicsBackend.OpenGLES:
                initializeOpenGL();
                break;

            case GraphicsBackend.Direct3D11:
                initializeDirect3D11();
                break;

            case GraphicsBackend.Metal:
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

        uint deviceExtPropCount = 0;
        VulkanNative.vkEnumerateDeviceExtensionProperties(vkDevice, (byte*)null, ref deviceExtPropCount, null);

        var deviceExtProps = new VkExtensionProperties[(int)deviceExtPropCount];
        VulkanNative.vkEnumerateDeviceExtensionProperties(vkDevice, (byte*)null, ref deviceExtPropCount, ref deviceExtProps[0]);

        uint instExtPropCount = 0;
        VulkanNative.vkEnumerateInstanceExtensionProperties((byte*)null, ref instExtPropCount, null);

        var instExtProps = new VkExtensionProperties[(int)instExtPropCount];
        VulkanNative.vkEnumerateInstanceExtensionProperties((byte*)null, ref instExtPropCount, ref instExtProps[0]);

        string extensions = string.Join(' ', instExtProps.Concat(deviceExtProps).Select(e => Marshal.PtrToStringUTF8((nint)e.extensionName)));

        Logger.Log($@"Vulkan Initialized");
        Logger.Log($@"Vulkan Device:         {deviceName}");
        Logger.Log($@"Vulkan API Version:    {apiVersion}");
        Logger.Log($@"Vulkan Driver Version: {driverVersion}");
        Logger.Log($@"Vulkan Extensions:     {extensions}");
    }

    private unsafe void initializeOpenGL()
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

        Logger.Log($@"GL Initialized");
        Logger.Log($@"GL Version:                 {version}");
        Logger.Log($@"GL Renderer:                {renderer}");
        Logger.Log($@"GL Shader Language Version: {shaderVersion}");
        Logger.Log($@"GL Vendor:                  {vendor}");
        Logger.Log($@"GL Extensions:              {extensions}");
    }

    private unsafe void initializeDirect3D11()
    {
        var info = Device.GetD3D11Info();
        var adapter = CppObject.FromPointer<Adapter>(info.Adapter);

        Logger.Log($@"Direct3D 11 Inititalized");
        Logger.Log($@"Direct3D 11 Adapter:                 {adapter.Description.Description}");
        Logger.Log($@"Direct3D 11 Dedicated Video Memory:  {adapter.Description.DedicatedVideoMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Dedicated System Memory: {adapter.Description.DedicatedSystemMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Shared System Memory:    {adapter.Description.SharedSystemMemory / 1024 / 1024} MB");
    }
}
