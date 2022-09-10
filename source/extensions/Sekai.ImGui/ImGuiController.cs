// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using Sekai.Engine;
using Sekai.Engine.Effects;
using Sekai.Engine.Effects.Compiler;
using Sekai.Engine.Graphics;
using Sekai.Framework.Annotations;
using Sekai.Framework.Graphics;
using Sekai.Framework.Storage;
using Gui = ImGuiNET.ImGui;

namespace Sekai.ImGui;

public class ImGuiController : GameSystem, IRenderable, IUpdateable
{
    [Resolved]
    private VirtualStorage storage { get; set; } = null!;

    [Resolved]
    private EffectCompiler compiler { get; set; } = null!;

    [Resolved]
    private IGraphicsDevice device { get; set; } = null!;

    internal Effect Effect { get; private set; } = null!;

    private ITexture? fontTexture;
    private ICommandQueue? queue;
    private GraphicsPipelineDescription pipelineDescriptor;
    private MutablePipeline pipelines = null!;
    private readonly ImGuiContext context;
    private readonly List<ImGuiPanel> panels = new();
    private const int texture_white_id = 0;
    private const int texture_atlas_id = 1;

    public ImGuiController()
    {
        pipelineDescriptor = new GraphicsPipelineDescription
        {
            Blend = new BlendStateDescription
            {
                Attachments = new[]
                {
                    new BlendAttachmentDescription
                    (
                        true,
                        null,
                        BlendFactor.One,
                        BlendFactor.One,
                        BlendFactor.Zero,
                        BlendFactor.Zero,
                        BlendFunction.Add,
                        BlendFunction.Add
                    )
                }
            },
            DepthStencil = new DepthStencilStateDescription
            {
                DepthTest = false,
                DepthWrite = false,
                DepthComparison = ComparisonKind.Always
            },
            Rasterizer = new RasterizerStateDescription
            {
                Culling = FaceCulling.None,
                Winding = FaceWinding.Clockwise,
                FillMode = PolygonFillMode.Solid,
            },
            Topology = PrimitiveTopology.Triangles,
        };

        context = new ImGuiContext();
    }

    protected override void Load()
    {
        using var stream = storage.Open("/engine/imgui/shaders/default.sksl", FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        Effect = compiler.Compile(new EffectSource(reader.ReadToEnd()), EffectType.Graphics);
        pipelines = new MutablePipeline(device);

        pipelineDescriptor.ShaderSet = new ShaderSetDescription
        (
            new[]
            {
                new VertexLayout()
                    .Add<Vector2>("Position")
                    .Add<Vector2>("TexCoord")
                    .Add<uint>("Column")
                    .Build()
            },
            Effect["Default"].Shaders,
            Array.Empty<ShaderConstant>()
        );

        pipelineDescriptor.Layouts = new[] { Effect["Default"].Layout };

        context.MakeCurrent();

        var io = Gui.GetIO();
        io.Fonts.AddFontDefault();
        io.Fonts.SetTexID((nint)texture_atlas_id);
        io.Fonts.GetTexDataAsRGBA32(out nint pixels, out int width, out int height, out int bpp);

        var textureDescriptor = new TextureDescription
        (
            (uint)width,
            (uint)height,
            1,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureKind.Texture2D,
            TextureUsage.Sampled,
            TextureSampleCount.Count1
        );

        fontTexture = device.Factory.CreateTexture(ref textureDescriptor);
        device.UpdateTextureData(fontTexture, pixels, (uint)(bpp * width * height), 0, 0, 0, (uint)width, (uint)height, 1, 0, 0);

        io.Fonts.ClearTexData();

        queue = device.Factory.CreateCommandQueue();
    }

    public void Update(double elapsed)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            var panel = panels[i];

            if (!panel.IsLoaded)
                continue;

            panel.Update(elapsed);
        }
    }

    public void Render()
    {
        if (queue is null)
            return;

        for (int i = 0; i < panels.Count; i++)
        {
            var panel = panels[i];

            if (!panel.IsLoaded)
                continue;

            queue.SetFramebuffer(panel.Framebuffer);
            panel.IndexBuffer.Bind(queue);
            panel.VertexBuffer.Bind(queue);

            var pipeline = pipelines.GetPipeline(pipelineDescriptor);
            queue.SetPipeline(pipeline);

            panel.MakeCurrent();

            int vOffset = 0;
            int iOffset = 0;
            var data = Gui.GetDrawData();

            for (int j = 0; j < data.CmdListsCount; j++)
            {
                var commands = data.CmdListsRange[j];

                for (int k = 0; k < commands.CmdBuffer.Size; k++)
                {
                    var buffer = commands.CmdBuffer[i];

                    if (buffer.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        if (buffer.TextureId == (nint)texture_white_id)
                        {
                            panel.SetActiveTexture(device.WhitePixel);
                        }
                        else if (buffer.TextureId == (nint)texture_atlas_id && fontTexture != null)
                        {
                            panel.SetActiveTexture(fontTexture);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    queue.SetResourceSet(0, panel.Resources);

                    queue.SetScissor
                    (
                        0,
                        new Rectangle
                        (
                            (int)buffer.ClipRect.X,
                            (int)buffer.ClipRect.Y,
                            (int)buffer.ClipRect.Z - (int)buffer.ClipRect.X,
                            (int)buffer.ClipRect.W - (int)buffer.ClipRect.Y
                        )
                    );

                    queue.DrawIndexed(buffer.ElemCount, 1, buffer.IdxOffset + (uint)iOffset, (int)buffer.VtxOffset + vOffset, 0);
                }
            }
        }
    }

    internal ImGuiContext CreateContext()
    {
        context.MakeCurrent();
        return new ImGuiContext(Gui.GetIO().Fonts);
    }
}
