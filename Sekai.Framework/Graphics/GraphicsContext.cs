// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sekai.Framework.Extensions;
using Sekai.Framework.Logging;
using Sekai.Framework.Platform;
using SharpDX;
using SharpDX.DXGI;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;
using Veldrid.OpenGLBinding;
using Vulkan;

namespace Sekai.Framework.Graphics;

internal class GraphicsContext : FrameworkObject, IGraphicsContext
{
    public readonly GraphicsDevice Device;
    public GraphicsAPI API { get; }
    public ResourceFactory Resources => Device.ResourceFactory;
    private readonly Dictionary<ResourceLayoutDescription, ResourceLayout> resourceLayoutCache = new();
    private readonly Dictionary<GraphicsPipelineState, Pipeline> graphicsPipelineCache = new();

    public bool VSync
    {
        get => Device.SyncToVerticalBlank;
        set => Device.SyncToVerticalBlank = value;
    }

    public GraphicsContext(GraphicsAPI api)
    {
        API = api;

        Device = api switch
        {
            GraphicsAPI.Direct3D11 => GraphicsDevice.CreateD3D11(new GraphicsDeviceOptions()),
            GraphicsAPI.Vulkan => GraphicsDevice.CreateVulkan(new GraphicsDeviceOptions()),
            GraphicsAPI.Metal => GraphicsDevice.CreateMetal(new GraphicsDeviceOptions()),
            GraphicsAPI.OpenGL => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(api))
        };

        initialize();
    }

    public GraphicsContext(IView view, GraphicsAPI api)
    {
        API = api;

        if (!view.IsInitialized)
            throw new InvalidOperationException(@"View must be initialized before a graphics context can be created.");

        Device = view.CreateGraphicsDevice
        (
            new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            }, api.ToVeldrid()
        );

        initialize();
    }

    internal Pipeline FetchPipeline(GraphicsPipelineState state)
    {
        lock (graphicsPipelineCache)
        {
            if (!graphicsPipelineCache.TryGetValue(state, out var pipeline))
            {
                var desc = new GraphicsPipelineDescription
                {
                    Outputs = state.Output.ToVeldrid(),
                    BlendState = state.Blending.ToVeldrid(),
                    ResourceLayouts = state.Shader.ResourceLayouts,
                    RasterizerState = state.Rasterizer.ToVeldrid(),
                    PrimitiveTopology = state.Rasterizer.Topology.ToVeldrid(),
                    ResourceBindingModel = ResourceBindingModel.Improved
                };

                desc.DepthStencilState.StencilBack = state.Stencil.Back.ToVeldrid();
                desc.DepthStencilState.StencilFront = state.Stencil.Front.ToVeldrid();
                desc.DepthStencilState.StencilReadMask = state.Stencil.ReadMask;
                desc.DepthStencilState.StencilWriteMask = state.Stencil.WriteMask;
                desc.DepthStencilState.StencilReference = (uint)state.Stencil.Reference;
                desc.DepthStencilState.StencilTestEnabled = state.Stencil.StencilTest;
                desc.DepthStencilState.DepthComparison = state.Depth.Comparison.ToVeldrid();
                desc.DepthStencilState.DepthTestEnabled = state.Depth.DepthTest;
                desc.DepthStencilState.DepthWriteEnabled = state.Depth.WriteDepth;
                desc.ShaderSet.VertexLayouts = new[] { state.Shader.VertexLayout };
                desc.ShaderSet.Shaders = state.Shader.Resources.ToArray();

                graphicsPipelineCache[state] = pipeline = Resources.CreateGraphicsPipeline(desc);
            }

            return pipeline;
        }
    }

    internal ResourceLayout FetchResourceLayout(ResourceLayoutDescription desc)
    {
        lock (resourceLayoutCache)
        {
            if (!resourceLayoutCache.TryGetValue(desc, out var layout))
            {
                resourceLayoutCache[desc] = layout = Resources.CreateResourceLayout(desc);
            }

            return layout;
        }
    }

    private void initialize()
    {
        switch (API)
        {
            case GraphicsAPI.Direct3D11:
                initializeDirect3D11();
                break;

            case GraphicsAPI.Vulkan:
                initializeVulkan();
                break;

            case GraphicsAPI.OpenGL:
                initializeGL();
                break;

            case GraphicsAPI.Metal:
            default:
                throw new NotSupportedException($"{API} is not supported.");
        }
    }

    private void initializeDirect3D11()
    {
        var info = Device.GetD3D11Info();
        var adapter = CppObject.FromPointer<Adapter>(info.Adapter);

        Logger.Log($@"Direct3D 11 Inititalized");
        Logger.Log($@"Direct3D 11 Adapter:                 {adapter.Description.Description}");
        Logger.Log($@"Direct3D 11 Dedicated Video Memory:  {adapter.Description.DedicatedVideoMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Dedicated System Memory: {adapter.Description.DedicatedSystemMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Shared System Memory:    {adapter.Description.SharedSystemMemory / 1024 / 1024} MB");
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

    private unsafe void initializeGL()
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

    protected sealed override void Destroy()
    {
        foreach ((var _, var pipeline) in graphicsPipelineCache)
            pipeline.Dispose();

        foreach ((var _, var layout) in resourceLayoutCache)
            layout.Dispose();

        Device.Dispose();
    }
}
