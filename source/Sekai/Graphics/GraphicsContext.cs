// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Allocation;
using Sekai.Extensions;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;

namespace Sekai.Graphics;

public sealed class GraphicsContext : DisposableObject
{
    /// <summary>
    /// The current depth state.
    /// </summary>
    public DepthInfo Depth
    {
        get => currentState.Depth;
        set
        {
            if (currentState.Depth.Equals(value))
                return;

            graphics.SetDepth(currentState.Depth = value);
        }
    }

    /// <summary>
    /// The current blending state.
    /// </summary>
    public BlendInfo Blend
    {
        get => currentState.Blend;
        set
        {
            if (currentState.Blend.Equals(value))
                return;

            graphics.SetBlend(currentState.Blend = value);
        }
    }

    /// <summary>
    /// The current stencil state.
    /// </summary>
    public StencilInfo Stencil
    {
        get => currentState.Stencil;
        set
        {
            if (currentState.Stencil.Equals(value))
                return;

            graphics.SetStencil(currentState.Stencil = value);
        }
    }

    /// <summary>
    /// The current scissor state.
    /// </summary>
    public ScissorInfo Scissor
    {
        get => currentState.Scissor;
        set
        {
            if (currentState.Scissor.Equals(value))
                return;

            graphics.SetScissor(currentState.Scissor = value);
        }
    }

    /// <summary>
    /// The current viewport state.
    /// </summary>
    public Viewport Viewport
    {
        get => currentState.Viewport;
        set
        {
            if (currentState.Viewport.Equals(value))
                return;

            graphics.SetViewport(currentState.Viewport = value);
        }
    }

    /// <summary>
    /// The current face winding state.
    /// </summary>
    public FaceWinding FaceWinding
    {
        get => currentState.FaceWinding;
        set
        {
            if (currentState.FaceWinding == value)
                return;

            graphics.SetFaceWinding(currentState.FaceWinding = value);
        }
    }

    /// <summary>
    /// The current face culling state.
    /// </summary>
    public FaceCulling FaceCulling
    {
        get => currentState.FaceCulling;
        set
        {
            if (currentState.FaceCulling == value)
                return;

            graphics.SetFaceCulling(currentState.FaceCulling = value);
        }
    }

    /// <summary>
    /// The default render target.
    /// </summary>
    public IRenderTarget DefaultRenderTarget => backBufferRenderTarget;

    /// <summary>
    /// The name of the current graphics device.
    /// </summary>
    public string Device => graphics.Device;

    /// <summary>
    /// The name of the current graphics system.
    /// </summary>
    public string System => graphics.Name;

    /// <summary>
    /// The version of the current graphics system.
    /// </summary>
    public Version Version => graphics.Version;

    /// <summary>
    /// A list of supported extensions the graphics device supports.
    /// </summary>
    public IReadOnlyList<string> Extensions => graphics.Extensions;

    private int stateStackDepth;
    private GraphicsState currentState;
    private BackBufferRenderTarget backBufferRenderTarget;
    private readonly GraphicsSystem graphics;
    private readonly Texture?[] textures = new Texture[16];
    private readonly Stack<GraphicsState> stateStack = new();
    private readonly Queue<Action> actions = new();
    private readonly GlobalUniform[] uniforms = new GlobalUniform[Enum.GetValues<GlobalUniforms>().Length];
    private readonly Dictionary<string, GlobalUniform> uniformMap = new();

    internal GraphicsContext(GraphicsSystem graphics)
    {
        this.graphics = graphics;
        addUniform(GlobalUniforms.Projection, Matrix4x4.Identity);
        addUniform(GlobalUniforms.Model, Matrix4x4.Identity);
        addUniform(GlobalUniforms.View, Matrix4x4.Identity);
    }

    internal void MakeCurrent() => graphics.MakeCurrent();

    internal void Present() => graphics.Present();

    internal GraphicsObject<NativeRenderTarget> CreateRenderTarget(IReadOnlyList<RenderBuffer> color, RenderBuffer? depth = null)
    {
        var target = graphics.CreateRenderTarget(color.Cast<NativeRenderBuffer>().ToArray(), depth);
        return new(enqueueOnDispose(target.Dispose), target);
    }

    internal GraphicsObject<NativeTexture> CreateTexture(int width, int height, int depth, int layers, int levels, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, PixelFormat format, TextureType type, TextureUsage usage, TextureSampleCount sampleCount)
    {
        var texture = graphics.CreateTexture(width, height, depth, levels, layers, min, mag, wrapModeS, wrapModeT, wrapModeR, type, usage, sampleCount, format);
        return new(enqueueOnDispose(texture.Dispose), texture);
    }

    internal GraphicsObject<NativeBuffer> CreateBuffer(int capacity, bool dynamic)
    {
        var buffer = graphics.CreateBuffer(capacity, dynamic);
        return new(enqueueOnDispose(buffer.Dispose), buffer);
    }

    internal GraphicsObject<NativeShader> CreateShader(string code)
    {
        var shader = graphics.CreateShader(code);

        foreach (var uniform in shader.Uniforms)
        {
            if (!uniformMap.TryGetValue(uniform.Name, out var uniformGlobal))
                continue;

            uniformGlobal.Attach(uniform);
        }

        var invoke = enqueueOnDispose(() =>
        {
            foreach (var uniform in shader.Uniforms)
            {
                if (!uniformMap.TryGetValue(uniform.Name, out var uniformGlobal))
                    continue;

                uniformGlobal.Detach(uniform);
            }

            shader.Dispose();
        });

        return new(invoke, shader);
    }

    private IDisposable enqueueOnDispose(Action dispose)
    {
        return new ValueInvokeOnDisposal(() => actions.Enqueue(dispose));
    }

    /// <summary>
    /// Prepares the graphics context for the next frame.
    /// </summary>
    internal void Prepare(Size2 size)
    {
        while (actions.TryDequeue(out var action))
            action();

        graphics.Prepare();

        foreach (var uniform in uniforms)
            uniform.Reset();

        stateStackDepth = 0;
        stateStack.Clear();

        currentState = new(backBufferRenderTarget = new(size));

        Depth = new(true);
        Blend = new(BlendingMask.None, new());
        Stencil = new(false);
        Scissor = new(true, new(0, 0, size.Width, size.Height));
        Viewport = new(0, 0, size.Width, size.Height, 0, 1);
        FaceCulling = FaceCulling.Back;
        FaceWinding = FaceWinding.CounterClockwise;

        stateStack.Push(currentState);

        Clear(new ClearInfo(Color4.Black));
    }

    /// <summary>
    /// Schedules an action to be invoked at the preparation step of the context.
    /// </summary>
    /// <param name="obj">The graphics object to dispose.</param>
    internal void Schedule(Action action) => actions.Enqueue(action);

    /// <summary>
    /// Clears the current framebuffer.
    /// </summary>
    public void Clear(ClearInfo info)
    {
        using (BeginContext())
        {
            Depth = new(writeDepth: true);
            Scissor = new(false);
            graphics.Clear(info);
        }
    }

    /// <summary>
    /// Begins a new context where the existing state is captured and restored after disposal.
    /// </summary>
    public IDisposable BeginContext()
    {
        Capture();
        return new ValueInvokeOnDisposal(Restore);
    }

    /// <summary>
    /// The captures the current graphics state.
    /// </summary>
    /// <remarks>
    /// The properties being captured and restored are as follows:
    /// <list type="bullet">
    /// <item>Depth</item>
    /// <item>Blend</item>
    /// <item>Scissor</item>
    /// <item>Stencil</item>
    /// <item>Viewport</item>
    /// <item>Face Winding</item>
    /// <item>Face Culling</item>
    /// <item>Shader</item>
    /// <item>Render Target</item>
    /// <item>Index Buffer</item>
    /// <item>Vertex Buffer</item>
    /// <item>Transform</item>
    /// </list>
    /// </remarks>
    public void Capture()
    {
        stateStack.Push(new(currentState));
        stateStackDepth++;
    }

    /// <summary>
    /// The restores the previous graphics state.
    /// </summary>
    public void Restore()
    {
        if (stateStackDepth <= 0 || !stateStack.TryPeek(out var prev))
            throw new InvalidOperationException(@"Cannot have more restores than captures.");

        Depth = prev.Depth;
        Blend = prev.Blend;
        Stencil = prev.Stencil;
        Scissor = prev.Scissor;
        Viewport = prev.Viewport;
        FaceWinding = prev.FaceWinding;
        FaceCulling = prev.FaceCulling;

        SetShader(prev.Shader);
        SetBuffer(prev.Index.Buffer, prev.Index.Format);
        SetBuffer(prev.Vertex.Buffer, prev.Vertex.Layout);
        SetTransform(prev.Transform.Model, prev.Transform.View, prev.Transform.Projection);
        SetRenderTarget(prev.Target);

        stateStackDepth--;
        stateStack.Pop();
    }

    /// <summary>
    /// Sets a texture at a given slot.
    /// </summary>
    /// <param name="texture">The texture to be set. If <see cref="null"/>, then the texture at that given slot is empty.</param>
    /// <param name="slot">The texture slot to be replaced.</param>
    public void SetTexture(Texture? texture, TextureUnit slot = 0)
    {
        if (!Enum.IsDefined(slot))
            throw new ArgumentOutOfRangeException(nameof(slot));

        if (ReferenceEquals(textures[(int)slot], texture))
            return;

        textures[(int)slot] = texture;
        graphics.SetTexture(textures[(int)slot]?.Native, (int)slot);
    }

    /// <summary>
    /// Sets the current render target where all draw operations will be blitted on to.
    /// </summary>
    /// <param name="target">The render target to be set.</param>
    public void SetRenderTarget(IRenderTarget target)
    {
        if (ReferenceEquals(currentState.Target, target))
            return;

        currentState.Target = target;
        graphics.SetRenderTarget(currentState.Target?.Native);
    }

    /// <summary>
    /// Sets the current shader used for drawing operations.
    /// </summary>
    /// <param name="shader">The shader to be set. If <see cref="null"/>, then the current shader is unbound.</param>
    public void SetShader(Shader? shader = null)
    {
        if (ReferenceEquals(currentState.Shader, shader))
            return;

        currentState.Shader = shader;
        graphics.SetShader(currentState.Shader?.Native);
    }

    /// <summary>
    /// Sets the current vertex buffer used for drawing operations.
    /// </summary>
    /// <param name="buffer">The buffer to be set.</param>
    /// <param name="layout">The vertex layout this buffer is assumed to be using.</param>
    public void SetBuffer(Buffers.Buffer? buffer = null, IVertexLayout? layout = null)
    {
        if (ReferenceEquals(currentState.Vertex.Buffer, buffer))
            return;

        currentState.Vertex = new(buffer, layout);
        graphics.SetBuffer(buffer?.Native, layout);
    }

    /// <summary>
    /// Sets the current index buffer used for drawing operations.
    /// </summary>
    /// <param name="buffer">The buffer to be set.</param>
    /// <param name="format">The index format this buffer is assumed to be using.</param>
    public void SetBuffer(Buffers.Buffer? buffer = null, IndexFormat format = IndexFormat.UInt16)
    {
        if (ReferenceEquals(currentState.Index.Buffer, buffer))
            return;

        currentState.Index = new(buffer, format);
        graphics.SetBuffer(buffer?.Native, format);
    }

    /// <summary>
    /// Sets the current vertex or index buffer whose binding type is determined from its type argument.
    /// </summary>
    /// <param name="buffer">The buffer to be set.</param>
    /// <exception cref="NotSupportedException">Thrown when the buffer binding type is neither a vertex or an index buffer.</exception>
    public void SetBuffer<T>(Buffer<T> buffer)
        where T : unmanaged
    {
        if (typeof(T).IsAssignableTo(typeof(IVertex)))
        {
            SetBuffer(buffer, VertexLayout.GetLayout(typeof(T)));
        }
        else if (supported_index_formats.TryGetValue(typeof(T), out var format))
        {
            SetBuffer(buffer, format);
        }
        else
        {
            throw new NotSupportedException(@"Cannot determine buffer binding type from given buffer.");
        }
    }

    /// <summary>
    /// Sets the current model, view, and projection matrices.
    /// </summary>
    /// <param name="model">The model matrix</param>
    /// <param name="view">The view matrix</param>
    /// <param name="projection">The projection matrix</param>
    public void SetTransform(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        if (model.Equals(currentState.Transform.Model) && view.Equals(currentState.Transform.View) && projection.Equals(currentState.Transform.Projection))
            return;

        setUniform(GlobalUniforms.View, currentState.Transform.View = view);
        setUniform(GlobalUniforms.Model, currentState.Transform.Model = model);
        setUniform(GlobalUniforms.Projection, currentState.Transform.Projection = projection);
    }

    /// <summary>
    /// Draws using the currently bound vertex buffer to the currently bound render target.
    /// </summary>
    /// <param name="vertexCount">The number of vertices to be drawn.</param>
    /// <param name="topology">The topology of the vertices.</param>
    /// <param name="instanceCount">The number of instances to be drawn.</param>
    /// <param name="baseVertex">The base vertex number for instanced rendering.</param>
    /// <param name="baseInstance">The base instance number for instanced rendering.</param>
    /// <exception cref="InvalidOperationException">Thrown when the context is in an invalid state.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when invalid arguments are specified.</exception>
    public void Draw(uint vertexCount, PrimitiveTopology topology, uint instanceCount = 1, int baseVertex = 0, uint baseInstance = 0)
    {
        if (currentState.Shader is null)
            throw new InvalidOperationException(@"A shader must be bound before draw operations can begin.");

        if (currentState.Vertex.Buffer is null)
            throw new InvalidOperationException(@"A vertex buffer must be bound before draw operations can begin.");

        graphics.Draw(vertexCount, topology, instanceCount, baseVertex, baseInstance);
    }

    /// <summary>
    /// Draws using the currently bound vertex and index buffer to the currently bound render target.
    /// </summary>
    /// <param name="indexCount">The number of indices to be used for drawing.</param>
    /// <param name="topology">The topology of the vertices.</param>
    /// <param name="instanceCount">The number of instances to be drawn.</param>
    /// <param name="indexOffset">The offset in the index buffer where to start.</param>
    /// <param name="baseVertex">The base vertex number for instanced rendering.</param>
    /// <param name="baseInstance">The base instance number for instanced rendering.</param>
    /// <exception cref="InvalidOperationException">Thrown when the context is in an invalid state.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when invalid arguments are specified.</exception>
    public void DrawIndexed(uint indexCount, PrimitiveTopology topology, uint instanceCount = 1, uint indexOffset = 0, int baseVertex = 0, uint baseInstance = 0)
    {
        if (currentState.Shader is null)
            throw new InvalidOperationException(@"A shader must be bound before draw operations can begin.");

        if (currentState.Vertex.Buffer is null || currentState.Index.Buffer is null)
            throw new InvalidOperationException(@"A vertex and index buffer must be bound before draw operations can begin.");

        graphics.DrawIndexed(indexCount, topology, instanceCount, indexOffset, baseVertex, baseInstance);
    }

    /// <summary>
    /// Draws using an indirect buffer to the currently bound render target.
    /// </summary>
    /// <param name="buffer">The indirect buffer to be used.</param>
    /// <param name="topology">The topology of the vertices</param>
    /// <param name="offset">The offset in the indirect buffer.</param>
    /// <param name="drawCount">The number of draw calls to be done.</param>
    /// <param name="stride">The stride of the indirect buffer.</param>
    /// <exception cref="InvalidOperationException">Thrown when the context is in an invalid state.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when invalid arguments are specified.</exception>
    public void DrawIndirect(Buffer<IndirectDrawArguments> buffer, PrimitiveTopology topology, uint offset = 0, uint drawCount = 1)
    {
        if (currentState.Shader is null)
            throw new InvalidOperationException(@"A shader must be bound before draw operations can begin.");

        if (currentState.Vertex.Buffer is null)
            throw new InvalidOperationException(@"A vertex buffer must be bound before draw operations can begin.");

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), @"Buffer offset cannot be less than 0.");

        if (drawCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(drawCount), @"Draw count cannot be less than 1.");

        graphics.DrawIndirect(buffer.Native, topology, offset, drawCount);
    }

    /// <summary>
    /// Draws using an indirect buffer to the currently bound render target.
    /// </summary>
    /// <param name="buffer">The indirect buffer to be used.</param>
    /// <param name="topology">The topology of the vertices</param>
    /// <param name="offset">The offset in the indirect buffer.</param>
    /// <param name="drawCount">The number of draw calls to be done.</param>
    /// <param name="stride">The stride of the indirect buffer.</param>
    /// <exception cref="InvalidOperationException">Thrown when the context is in an invalid state.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when invalid arguments are specified.</exception>
    public void DrawIndexedIndirect(Buffer<IndirectDrawArguments> buffer, PrimitiveTopology topology, uint offset = 0, uint drawCount = 1)
    {
        if (currentState.Shader is null)
            throw new InvalidOperationException(@"A shader must be bound before draw operations can begin.");

        if (currentState.Vertex.Buffer is null || currentState.Index.Buffer is null)
            throw new InvalidOperationException(@"A vertex and index buffer must be bound before draw operations can begin.");

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), @"Buffer offset cannot be less than 0.");

        if (drawCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(drawCount), @"Draw count cannot be less than 1.");

        graphics.DrawIndexedIndirect(buffer.Native, topology, offset, drawCount);
    }

    private void setUniform<T>(GlobalUniforms key, T value)
        where T : unmanaged, IEquatable<T>
    {
        ((GlobalUniform<T>)uniforms[(int)key]).Value = value;
    }

    private void addUniform<T>(GlobalUniforms key, T defaultValue = default)
        where T : unmanaged, IEquatable<T>
    {
        string name = key.GetDescription();
        uniformMap.Add(name, uniforms[(int)key] = new GlobalUniform<T>(name, defaultValue));
    }

    private static readonly Dictionary<Type, IndexFormat> supported_index_formats = new()
    {
        { typeof(int), IndexFormat.UInt32 },
        { typeof(uint), IndexFormat.UInt32 },
        { typeof(short), IndexFormat.UInt16 },
        { typeof(ushort), IndexFormat.UInt16 },
    };

    private struct GraphicsState
    {
        public DepthInfo Depth;
        public BlendInfo Blend;
        public ScissorInfo Scissor;
        public StencilInfo Stencil;
        public Viewport Viewport;
        public FaceWinding FaceWinding;
        public FaceCulling FaceCulling;
        public Shader? Shader;
        public IRenderTarget Target;
        public IndexBufferInfo Index;
        public VertexBufferInfo Vertex;
        public TransformInfo Transform;

        public GraphicsState(BackBufferRenderTarget backBufferRenderTarget)
        {
            Target = backBufferRenderTarget;
        }

        public GraphicsState(GraphicsState state)
        {
            Depth = state.Depth;
            Blend = state.Blend;
            Scissor = state.Scissor;
            Stencil = state.Stencil;
            Viewport = state.Viewport;
            FaceWinding = state.FaceWinding;
            FaceCulling = state.FaceCulling;
            Shader = state.Shader;
            Target = state.Target;
            Index = state.Index;
            Vertex = state.Vertex;
        }
    }

    private struct TransformInfo
    {
        public Matrix4x4 Model;
        public Matrix4x4 View;
        public Matrix4x4 Projection;
    }

    private struct IndexBufferInfo
    {
        public IndexFormat Format;
        public Buffers.Buffer? Buffer;

        public IndexBufferInfo(Buffers.Buffer? buffer, IndexFormat format)
        {
            Buffer = buffer;
            Format = format;
        }
    }

    private struct VertexBufferInfo
    {
        public IVertexLayout? Layout;
        public Buffers.Buffer? Buffer;

        public VertexBufferInfo(Buffers.Buffer? buffer, IVertexLayout? layout)
        {
            Buffer = buffer;
            Layout = layout;
        }
    }

    private readonly struct BackBufferRenderTarget : IRenderTarget
    {
        public int Width { get; }
        public int Height { get; }

        public BackBufferRenderTarget(Size2 size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        NativeRenderTarget? IRenderTarget.Native => null;
    }
}
