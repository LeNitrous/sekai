// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.Versioning;
using Sekai.Platform;
using Sekai.Platform.Android;
using Sekai.Platform.IOS;
using Sekai.Platform.Linux;
using Sekai.Platform.MacOS;
using Sekai.Platform.OpenGL;
using Sekai.Platform.Windows;
using VdGraphicsDevice = Veldrid.GraphicsDevice;
using VdPixelFormat = Veldrid.PixelFormat;
using VdResourceBindingModel = Veldrid.ResourceBindingModel;
using VdSwapchainSource = Veldrid.SwapchainSource;

namespace Sekai.Graphics;

/// <summary>
/// Represents a physical graphics device.
/// </summary>
public sealed class GraphicsDevice : IDisposable
{
    /// <summary>
    /// The graphics API this device is using.
    /// </summary>
    public GraphicsAPI API { get; }

    /// <summary>
    /// The graphics API version.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// The name of this graphics device.
    /// </summary>
    public string Name => Veldrid.DeviceName;

    /// <summary>
    /// The name of this graphics device's vendor.
    /// </summary>
    public string Vendor => Veldrid.VendorName;

    /// <summary>
    /// The Veldrid Graphics Device.
    /// </summary>
    internal VdGraphicsDevice Veldrid { get; }

    /// <summary>
    /// The Veldrid Resource Factory.
    /// </summary>
    internal Veldrid.ResourceFactory Factory => Veldrid.ResourceFactory;

    private bool isDisposed;

    /// <summary>
    /// Creates a new instance of a graphics device.
    /// </summary>
    /// <param name="veldrid">The Veldrid graphics device.</param>
    internal GraphicsDevice(VdGraphicsDevice veldrid, GraphicsAPI api)
    {
        API = api;
        Veldrid = veldrid;
        Version = new(Veldrid.ApiVersion.Major, Veldrid.ApiVersion.Minor, Veldrid.ApiVersion.Patch, Veldrid.ApiVersion.Subminor);
    }

    /// <summary>
    /// Creates a graphics device without a main swapchain using a preferred graphics API.
    /// </summary>
    /// <returns>A new graphics device.</returns>
    public static GraphicsDevice Create()
    {
        return Create(RuntimeInfo.GetPreferredGraphicsAPI());
    }

    /// <summary>
    /// Creates a graphics device without a main swapchain.
    /// </summary>
    /// <param name="api">The graphics API to use.</param>
    /// <returns>A new grapics device.</returns>
    public static GraphicsDevice Create(GraphicsAPI api)
    {
        VdGraphicsDevice device;

        switch (api)
        {
            case GraphicsAPI.D3D11 when RuntimeInfo.IsWindows:
                device = VdGraphicsDevice.CreateD3D11(default_options);
                break;

            case GraphicsAPI.Metal when RuntimeInfo.IsApple:
                device = VdGraphicsDevice.CreateMetal(default_options);
                break;

            case GraphicsAPI.Vulkan:
                device = VdGraphicsDevice.CreateVulkan(default_options);
                break;

            case GraphicsAPI.OpenGL:
                throw new NotSupportedException("An OpenGL device cannot be created without a main swapchain.");

            default:
                throw new PlatformNotSupportedException();
        }

        return new(device, api);
    }

    /// <summary>
    /// Creates a graphics device with a main swapchain using a preferred graphics API.
    /// </summary>
    /// <param name="surface">The graphics API to use.</param>
    /// <param name="swapchain">The created swapchain.</param>
    /// <returns>A new graphics device.</returns>
    public static GraphicsDevice Create(ISurface surface, out Veldrid.Swapchain swapchain)
    {
        return Create(RuntimeInfo.GetPreferredGraphicsAPI(), surface, out swapchain);
    }

    /// <summary>
    /// Creates a graphics device with a main swapchain.
    /// </summary>
    /// <param name="api">The graphics API to use.</param>
    /// <param name="surface">The surface to use with the swapchain.</param>
    /// <param name="swapchain">The created swapchain.</param>
    /// <returns>A new graphics device.</returns>
    public static GraphicsDevice Create(GraphicsAPI api, ISurface surface, out Veldrid.Swapchain swapchain)
    {
        VdGraphicsDevice device;

        var descriptor = new Veldrid.SwapchainDescription
        {
            Width = (uint)surface.Size.Width,
            Height = (uint)surface.Size.Height,
            ColorSrgb = true,
            SyncToVerticalBlank = true,
        };

        switch (api)
        {
            case GraphicsAPI.D3D11 when RuntimeInfo.IsWindows:
                device = createD3D11Device(surface, descriptor);
                break;

            case GraphicsAPI.Metal when RuntimeInfo.IsApple:
                device = createMetalDevice(surface, descriptor);
                break;

            case GraphicsAPI.Vulkan:
                device = createVulkanDevice(surface, descriptor);
                break;

            case GraphicsAPI.OpenGL when RuntimeInfo.IsMobile:
                device = createGLESDevice(surface, descriptor);
                break;

            case GraphicsAPI.OpenGL:
                device = createGLDevice(surface);
                break;

            default:
                throw new PlatformNotSupportedException();
        }

        swapchain = device.MainSwapchain;

        return new(device, api);
    }

    [SupportedOSPlatform("windows")]
    private static VdGraphicsDevice createD3D11Device(ISurface surface, Veldrid.SwapchainDescription descriptor)
    {
        if (surface is IWin32Window win32)
        {
            descriptor.Source = VdSwapchainSource.CreateWin32(win32.Handle, win32.Instance);
        }

        if (surface is IWinRTSwapChainPanel winrt)
        {
            descriptor.Source = VdSwapchainSource.CreateUwp(winrt.Panel, winrt.LogicalDPI);
        }

        ensureValidDescriptor(descriptor);

        return VdGraphicsDevice.CreateD3D11(default_options, descriptor);
    }

    [SupportedOSPlatform("osx")]
    [SupportedOSPlatform("maccatalyst")]
    private static VdGraphicsDevice createMetalDevice(ISurface surface, Veldrid.SwapchainDescription descriptor)
    {
        if (surface is INSView nsView)
        {
            descriptor.Source = VdSwapchainSource.CreateNSView(nsView.Handle);
        }

        if (surface is INSWindow nsWindow)
        {
            descriptor.Source = VdSwapchainSource.CreateNSWindow(nsWindow.Handle);
        }

        if (surface is IUIView uiView)
        {
            descriptor.Source = VdSwapchainSource.CreateUIView(uiView.Handle);
        }

        ensureValidDescriptor(descriptor);

        return VdGraphicsDevice.CreateMetal(default_options, descriptor);
    }

    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("maccatalyst")]
    private static VdGraphicsDevice createGLESDevice(ISurface surface, Veldrid.SwapchainDescription descriptor)
    {
        if (surface is IUIView uiView)
        {
            descriptor.Source = VdSwapchainSource.CreateUIView(uiView.Handle);
        }

        if (surface is IAndroidSurface android)
        {
            descriptor.Source = VdSwapchainSource.CreateAndroidSurface(android.Surface, android.JNIHandle);
        }

        ensureValidDescriptor(descriptor);

        return VdGraphicsDevice.CreateOpenGLES(default_options, descriptor);
    }

    private static VdGraphicsDevice createGLDevice(ISurface surface)
    {
        if (surface is not IGLContextSource source)
        {
            throw new ArgumentException("The surface cannot provide GL contexts.", nameof(surface));
        }

        var info = new Veldrid.OpenGL.OpenGLPlatformInfo
        (
            source.CreateContext(),
            source.GetProcAddress,
            source.MakeCurrent,
            source.GetCurrentContext,
            source.ClearCurrentContext,
            source.DeleteContext,
            source.SwapBuffers,
            source.SetSyncToVerticalBlank
        );

        return VdGraphicsDevice.CreateOpenGL(default_options, info, (uint)surface.Size.Width, (uint)surface.Size.Height);
    }

    private static VdGraphicsDevice createVulkanDevice(ISurface surface, Veldrid.SwapchainDescription descriptor)
    {
        descriptor.Source = surface switch
        {
            IWinRTSwapChainPanel winrt => VdSwapchainSource.CreateUwp(winrt.Panel, winrt.LogicalDPI),
            IWin32Window win32 => VdSwapchainSource.CreateWin32(win32.Handle, win32.Instance),
            IWaylandWindow wayland when wayland.IsWayland => VdSwapchainSource.CreateWayland(wayland.Display, wayland.Surface),
            IXLibWindow xLibWindow when xLibWindow.IsXLib => VdSwapchainSource.CreateXlib(xLibWindow.Display, xLibWindow.Surface),
            INSWindow nsWindow => VdSwapchainSource.CreateNSWindow(nsWindow.Handle),
            INSView nsView => VdSwapchainSource.CreateNSView(nsView.Handle),
            IUIView uiView => VdSwapchainSource.CreateUIView(uiView.Handle),
            IAndroidSurface android => VdSwapchainSource.CreateAndroidSurface(android.Surface, android.JNIHandle),
            _ => throw new NotSupportedException(),
        };

        ensureValidDescriptor(descriptor);

        return VdGraphicsDevice.CreateVulkan(default_options, descriptor);
    }

    private static void ensureValidDescriptor(Veldrid.SwapchainDescription descriptor)
    {
        if (descriptor.Source is null)
        {
            throw new InvalidOperationException("Swapchain descriptor does not have a source.");
        }
    }

    ~GraphicsDevice()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Veldrid.Dispose();
        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    private static readonly Veldrid.GraphicsDeviceOptions default_options = new
    (
        RuntimeInfo.IsDebug,
        VdPixelFormat.R8_G8_B8_A8_UNorm_SRgb,
        true,
        VdResourceBindingModel.Improved
    );
}
