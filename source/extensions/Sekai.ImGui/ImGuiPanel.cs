// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Engine;
using Sekai.Engine.Graphics;
using Sekai.Framework.Allocation;
using Sekai.Framework.Annotations;
using Sekai.Framework.Graphics;
using Gui = ImGuiNET.ImGui;

namespace Sekai.ImGui;

public class ImGuiPanel : LoadableObject, IUpdateable
{
    public Size Size
    {
        get => size;
        set
        {
            if (size == value)
                return;

            size = value;
            resizeRequested = true;
        }
    }

    public int Width
    {
        get => size.Width;
        set
        {
            if (Size.Width == value)
                return;

            Size = new Size(value, Size.Height);
        }
    }

    public int Height
    {
        get => Size.Height;
        set
        {
            if (Size.Height == value)
                return;

            Size = new Size(Size.Width, value);
        }
    }

    internal IResourceSet Resources
    {
        get
        {
            if (resourcesChanged)
                invalidateResourceSet();

            return resources;
        }
    }

    public ITexture Texture { get; private set; } = null!;
    internal IndexBuffer IndexBuffer { get; private set; } = null!;
    internal IFramebuffer Framebuffer { get; private set; } = null!;
    internal VertexBuffer VertexBuffer { get; private set; } = null!;

    [Resolved]
    private IGraphicsDevice device { get; set; } = null!;

    [Resolved]
    private ImGuiController controller { get; set; } = null!;

    private Size size;
    private ImGuiContext? context;
    private IBuffer uniforms = null!;
    private ITexture activeTexture = null!;
    private IResourceSet resources = null!;
    private bool resizeRequested = true;
    private bool resourcesChanged = true;

    protected override void Load()
    {
        var bufferDescriptor = new BufferDescription(64, BufferUsage.Uniform);
        uniforms = device.Factory.CreateBuffer(ref bufferDescriptor);
        context = controller.CreateContext();
    }

    public void Update(double elapsed)
    {
        MakeCurrent();

        Gui.NewFrame();

        var io = Gui.GetIO();
        io.DeltaTime = (float)elapsed;
        io.DisplaySize = new Vector2(Width, Height);
        io.DisplayFramebufferScale = Vector2.One;

        Gui.Render();

        var data = Gui.GetDrawData();

        if (VertexBuffer is null || VertexBuffer.Size < data.TotalVtxCount * Marshal.SizeOf<ColoredVertex>())
        {
            VertexBuffer?.Dispose();
            VertexBuffer = new VertexBuffer<ColoredVertex>(device, data.TotalVtxCount);
        }

        if (IndexBuffer is null || IndexBuffer.Size < data.TotalIdxCount * sizeof(ushort))
        {
            IndexBuffer?.Dispose();
            IndexBuffer = new IndexBuffer<ushort>(device, data.TotalIdxCount);
        }

        int vOffset = 0;
        int iOffset = 0;

        for (int i = 0; i < data.CmdListsCount; i++)
        {
            var commands = data.CmdListsRange[i];

            IndexBuffer.SetData(commands.IdxBuffer.Data, commands.IdxBuffer.Size * sizeof(ushort), iOffset * sizeof(ushort));
            VertexBuffer.SetData(commands.VtxBuffer.Data, commands.VtxBuffer.Size * Marshal.SizeOf<ColoredVertex>(), vOffset * Marshal.SizeOf<ColoredVertex>());

            iOffset += commands.IdxBuffer.Size;
            vOffset += commands.VtxBuffer.Size;
        }

        if (resizeRequested)
        {
            Framebuffer?.Dispose();
            Texture?.Dispose();

            var textureDescriptor = new TextureDescription
            (
                (uint)Size.Width,
                (uint)Size.Height,
                1,
                0,
                0,
                PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
                TextureKind.Texture2D,
                TextureUsage.RenderTarget | TextureUsage.Sampled,
                TextureSampleCount.Count1
            );

            Texture = device.Factory.CreateTexture(ref textureDescriptor);

            var framebufferDescriptor = new FramebufferDescription
            (
                null,
                new[]
                {
                    new FramebufferAttachment(device.Factory.CreateTexture(ref textureDescriptor), 0, 0)
                }
            );

            Framebuffer = device.Factory.CreateFramebuffer(ref framebufferDescriptor);

            resizeRequested = false;
        }

        var mvp = Matrix4x4.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1.0f, 1.0f);
        device.UpdateBufferData(uniforms, ref mvp, 0);
    }

    internal void MakeCurrent()
    {
        context?.MakeCurrent();
    }

    internal void SetActiveTexture(ITexture texture)
    {
        activeTexture = texture;
        resourcesChanged = true;
    }

    private void invalidateResourceSet()
    {
        resources?.Dispose();

        var descriptor = new ResourceSetDescription
        (
            controller.Effect["Default"].Layout,
            new IBindableResource[]
            {
                uniforms,
                activeTexture,
                device.SamplerPoint,
            }
        );

        resources = device.Factory.CreateResourceSet(ref descriptor);

        resourcesChanged = false;
    }

    protected override void Unload()
    {
        context?.Dispose();
    }
}
