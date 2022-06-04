// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Sekai.Framework.Logging;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;
using Vulkan;

namespace Sekai.Framework.Graphics;

public class GraphicsContext : FrameworkComponent, IGraphicsContext
{
    public IView View { get; }
    public CommandList? Commands { get; private set; }
    public GraphicsDevice? Device { get; private set; }

    [MemberNotNullWhen(true, nameof(Device), nameof(Commands))]
    public bool IsLoaded { get; private set; }

    private readonly GraphicsBackend graphicsAPI = GraphicsBackend.Vulkan;

    public GraphicsContext()
    {
        var viewOptions = new ViewOptions
        {
            API = graphicsAPI.ToGraphicsAPI(),
            VSync = false,
            FramesPerSecond = 120,
            UpdatesPerSecond = 240,
            ShouldSwapAutomatically = false,
        };

        var windowOptions = new WindowOptions(viewOptions)
        {
            Size = new Vector2D<int>(1366, 768),
            Title = @"Sekai Framework",
            WindowBorder = WindowBorder.Fixed,
        };

        View = Window.IsViewOnly
            ? Window.GetView(viewOptions)
            : Window.Create(windowOptions);

        View.Load += initialize;
        View.Resize += size => Device?.ResizeMainWindow((uint)size.X, (uint)size.Y);
    }

    private unsafe void initialize()
    {
        Device = View.CreateGraphicsDevice
        (
            new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            }, graphicsAPI
        );

        Commands = Device.ResourceFactory.CreateCommandList();

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
