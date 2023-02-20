// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Graphics.Shaders;
using Sekai.Mathematics;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using SkiaSharp;

namespace Sekai.Skia;

internal class SkiaContext : ServiceableObject
{
    [Resolved]
    private Surface surface { get; set; } = null!;

    [Resolved]
    private AssetLoader content { get; set; } = null!;

    private Shader? shader;
    private readonly GRContext? context;

    public SkiaContext()
    {
        if (surface is IOpenGLContextSource source)
            context = GRContext.CreateGl(GRGlInterface.Create(source.GetProcAddress));
    }

    public Shader GetShader() => shader ??= content.Load<Shader>("./engine/shaders/batches/quad.sksl");

    public SKSurface CreateSurface(Size2 size) => context is not null
        ? SKSurface.Create(context, false, new(size.Width, size.Height))
        : SKSurface.Create(new SKImageInfo(size.Width, size.Height));

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            context?.Dispose();

        base.Dispose(disposing);
    }
}
