// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Sekai.Framework.Logging;
using Veldrid;
using Vulkan;

namespace Sekai.Framework.Graphics;

internal sealed class VulkanGraphicsContext : GraphicsContext
{
    protected override GraphicsBackend Backend => GraphicsBackend.Vulkan;

    protected override unsafe void Initialize()
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
}
