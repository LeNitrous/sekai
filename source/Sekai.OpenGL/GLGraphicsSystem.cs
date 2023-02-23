// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.OpenGL.Extensions;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using Silk.NET.OpenGL;
using SekaiFormat = Sekai.Graphics.Textures.PixelFormat;

namespace Sekai.OpenGL;

internal unsafe class GLGraphicsSystem : GraphicsSystem
{
    public override string Name { get; } = @"OpenGL";

    public override Version Version { get; }

    public override string Device { get; }

    public override IReadOnlyList<string> Extensions { get; }

    private uint enabledAttributeCount;
    private Viewport currentViewport;
    private GLShader? currentShader;
    private DrawElementsType indexFormat;
    private readonly GL api;
    private readonly uint vao;
    private readonly IOpenGLContext context;
    private readonly IOpenGLContextSource source;
    private readonly Queue<Action> deferredActionQueue = new();

    public GLGraphicsSystem(Surface surface)
        : base(surface)
    {
        if (surface is not IOpenGLContextSource source)
            throw new ArgumentException($"Surface must implement {nameof(IOpenGLContextSource)}.");

        this.source = source;

        context = source.CreateContext();
        context.MakeCurrent();

        api = GL.GetApi(source.GetProcAddress);
        api.DebugMessageCallback(throwOnError, null);

        vao = api.GenVertexArray();
        api.BindVertexArray(vao);

        Device = $"{api.GetStringS(StringName.Vendor)} {api.GetStringS(StringName.Renderer)}";

        api.GetInteger(GetPName.MajorVersion, out int major);
        api.GetInteger(GetPName.MinorVersion, out int minor);

        Version = new Version(major, minor);

        api.GetInteger(GetPName.NumExtensions, out int num);
        var extensions = new List<string>(num);

        for (uint i = 0; i < num; i++)
            extensions.Add(api.GetStringS(StringName.Extensions, i));

        Extensions = extensions;

        source.ClearCurrentContext();
    }

    private static void throwOnError(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint data, nint userdata)
    {
        if (type == GLEnum.DebugTypeError)
            throw new GLException($@"An OpenGL exception has occured: {Encoding.UTF8.GetString((byte*)data, length)}");
    }

    protected override ShaderTranspiler CreateShaderTranspiler() => new GLShaderTranspiler();

    public override void Present()
    {
        source.SwapBuffers();
    }

    public override void Prepare()
    {
        while (deferredActionQueue.TryDequeue(out var action))
            action();
    }

    public override void MakeCurrent()
    {
        context.MakeCurrent();
    }

    public override void Clear(ClearInfo info)
    {
        api.ClearColor(info.Color.R, info.Color.G, info.Color.B, info.Color.A);
        api.ClearDepth(info.Depth);
        api.ClearStencil(info.Stencil);
        api.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }

    public override void SetDepth(DepthInfo info)
    {
        if (info.DepthTest)
        {
            api.Enable(EnableCap.DepthTest);
            api.DepthFunc(GLUtils.ToDepthFunction(info.Function));
        }
        else
        {
            api.Disable(EnableCap.DepthTest);
        }

        api.DepthMask(info.WriteDepth);
    }

    public override void SetScissor(ScissorInfo scissor)
    {
        if (scissor.Enabled)
        {
            api.Enable(EnableCap.ScissorTest);
            api.Scissor(scissor.Rectangle.X, currentViewport.Height - scissor.Rectangle.Bottom, (uint)scissor.Rectangle.Width, (uint)scissor.Rectangle.Height);
        }
        else
        {
            api.Disable(EnableCap.ScissorTest);
        }
    }

    public override void SetStencil(StencilInfo info)
    {
        if (info.StencilTest)
        {
            api.Enable(EnableCap.StencilTest);
            api.StencilFunc(GLUtils.ToStencilFunction(info.Function), info.Value, (uint)info.Mask);
            api.StencilOp
            (
                GLUtils.ToStencilOperation(info.StencilFailOperation),
                GLUtils.ToStencilOperation(info.DepthTestFailOperation),
                GLUtils.ToStencilOperation(info.PassOperation)
            );
        }
        else
        {
            api.Disable(EnableCap.StencilTest);
        }
    }

    public override void SetViewport(Viewport viewport)
    {
        currentViewport = viewport;
        api.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
        api.DepthRange(viewport.MinimumDepth, viewport.MaximumDepth);
    }

    public override void SetBlend(BlendInfo blend)
    {
        if (blend.Parameters.IsDisabled)
        {
            api.Disable(EnableCap.Blend);
        }
        else
        {
            api.Enable(EnableCap.Blend);

            api.BlendEquationSeparate
            (
                GLUtils.ToBlendEquationModeEXT(blend.Parameters.ColorEquation),
                GLUtils.ToBlendEquationModeEXT(blend.Parameters.AlphaEquation)
            );

            api.BlendFuncSeparate
            (
                GLUtils.ToBlendingFactor(blend.Parameters.SourceColor),
                GLUtils.ToBlendingFactor(blend.Parameters.DestinationColor),
                GLUtils.ToBlendingFactor(blend.Parameters.SourceAlpha),
                GLUtils.ToBlendingFactor(blend.Parameters.DestinationAlpha)
            );

            api.ColorMask
            (
                blend.Masking.HasFlag(BlendingMask.Red),
                blend.Masking.HasFlag(BlendingMask.Green),
                blend.Masking.HasFlag(BlendingMask.Blue),
                blend.Masking.HasFlag(BlendingMask.Alpha)
            );
        }
    }

    public override void SetFaceCulling(FaceCulling culling)
    {
        if (culling == FaceCulling.None)
        {
            api.Disable(EnableCap.CullFace);
        }
        else
        {
            api.Enable(EnableCap.CullFace);

            switch (culling)
            {
                case FaceCulling.Back:
                    api.CullFace(CullFaceMode.Back);
                    break;

                case FaceCulling.Front:
                    api.CullFace(CullFaceMode.Front);
                    break;

                case FaceCulling.Both:
                    api.CullFace(CullFaceMode.FrontAndBack);
                    break;
            }
        }
    }

    public override void SetFaceWinding(FaceWinding winding)
    {
        switch (winding)
        {
            case FaceWinding.Clockwise:
                api.FrontFace(FrontFaceDirection.CW);
                break;

            case FaceWinding.CounterClockwise:
                api.FrontFace(FrontFaceDirection.Ccw);
                break;
        }
    }

    public override void SetTexture(NativeTexture? texture, int unit)
    {
        uint id = 0;
        var target = TextureTarget.Texture2D;

        if (texture is GLTexture t)
        {
            id = t;
            target = t.Target;
        }

        api.ActiveTexture(Silk.NET.OpenGL.TextureUnit.Texture0 + unit);
        api.BindTexture(target, id);
    }

    public override void SetShader(NativeShader? shader)
    {
        currentShader = shader as GLShader;

        if (currentShader is not null)
        {
            api.UseProgram(currentShader);

            foreach (var uniform in currentShader.Uniforms)
                UpdateShaderUniform((GLUniform)uniform);
        }
        else
        {
            api.UseProgram(0);
        }
    }

    public override void SetBuffer(NativeBuffer? buffer, IndexFormat format)
    {
        indexFormat = format switch
        {
            IndexFormat.UInt16 => DrawElementsType.UnsignedShort,
            IndexFormat.UInt32 => DrawElementsType.UnsignedInt,
            _ => throw new NotSupportedException($@"Index format ""{indexFormat}"" is not supported.")
        };

        api.BindBuffer(BufferTargetARB.ElementArrayBuffer, buffer is not null ? (GLBuffer)buffer : 0);
    }

    public override unsafe void SetBuffer(NativeBuffer? buffer, IVertexLayout? layout)
    {
        if (buffer is null || layout is null)
        {
            for (uint i = 0; i < enabledAttributeCount; i++)
                api.DisableVertexAttribArray(i);

            enabledAttributeCount = 0;

            api.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        }
        else
        {
            if (layout.Members.Count > enabledAttributeCount)
            {
                for (uint i = enabledAttributeCount; i < layout.Members.Count; i++)
                    api.EnableVertexAttribArray(i);
            }
            else
            {
                for (uint i = enabledAttributeCount - 1; i >= layout.Members.Count; i++)
                    api.DisableVertexAttribArray(i);
            }

            enabledAttributeCount = (uint)layout.Members.Count;

            api.BindBuffer(BufferTargetARB.ArrayBuffer, (GLBuffer)buffer);

            for (int i = 0; i < layout.Members.Count; i++)
            {
                var member = layout.Members[i];
                api.VertexAttribPointer((uint)i, member.Count, member.Format.ToVertexAttribPointerType(), member.Normalized, (uint)layout.Stride, (void*)member.Offset);
            }
        }
    }

    public override void SetRenderTarget(NativeRenderTarget? target)
    {
        api.BindFramebuffer(FramebufferTarget.Framebuffer, target is not null ? (GLRenderTarget)target : 0);
    }

    public override void Draw(uint vertexCount, PrimitiveTopology topology, uint instanceCount, int baseVertex, uint baseInstance)
    {
        var primitive = GLUtils.ToPrimitiveType(topology);

        if (instanceCount == 1 && baseInstance == 0)
        {
            api.DrawArrays(primitive, baseVertex, vertexCount);
        }
        else
        {
            if (baseInstance == 0)
            {
                api.DrawArraysInstanced(primitive, baseVertex, vertexCount, instanceCount);
            }
            else
            {
                api.DrawArraysInstancedBaseInstance(primitive, baseVertex, vertexCount, instanceCount, baseInstance);
            }
        }
    }

    public override unsafe void DrawIndexed(uint indexCount, PrimitiveTopology topology, uint instanceCount, uint indexOffset, int baseVertex, uint baseInstance)
    {
        var primitive = GLUtils.ToPrimitiveType(topology);

        uint indexSize = indexFormat == DrawElementsType.UnsignedShort ? 2u : 4u;
        nint indices = (nint)(indexOffset * indexSize);

        if (instanceCount == 1 && baseInstance == 0)
        {
            if (baseVertex == 0)
            {
                api.DrawElements(primitive, indexCount, indexFormat, (void*)indices);
            }
            else
            {
                api.DrawElementsBaseVertex(primitive, indexCount, indexFormat, (void*)indices, baseVertex);
            }
        }
        else
        {
            if (baseInstance > 0)
            {
                api.DrawElementsInstancedBaseVertexBaseInstance(primitive, indexCount, indexFormat, (void*)indices, instanceCount, baseVertex, baseInstance);
            }
            else if (baseVertex == 0)
            {
                api.DrawElementsInstanced(primitive, indexCount, indexFormat, (void*)indices, instanceCount);
            }
            else
            {
                api.DrawElementsInstancedBaseVertex(primitive, indexCount, indexFormat, (void*)indices, instanceCount, baseVertex);
            }
        }
    }

    public override unsafe void DrawIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount)
    {
        api.BindBuffer(BufferTargetARB.DrawIndirectBuffer, (GLBuffer)buffer);

        if (drawCount == 1)
        {
            api.DrawArraysIndirect(GLUtils.ToPrimitiveType(topology), (void*)offset);
        }
        else
        {
            api.MultiDrawArraysIndirect(GLUtils.ToPrimitiveType(topology), (void*)offset, drawCount, (uint)Unsafe.SizeOf<IndirectDrawArguments>());
        }
    }

    public override unsafe void DrawIndexedIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount)
    {
        api.BindBuffer(BufferTargetARB.DrawIndirectBuffer, (GLBuffer)buffer);

        if (drawCount == 1)
        {
            api.DrawArraysIndirect(GLUtils.ToPrimitiveType(topology), (void*)offset);
        }
        else
        {
            api.MultiDrawArraysIndirect(GLUtils.ToPrimitiveType(topology), (void*)offset, drawCount, (uint)Unsafe.SizeOf<IndirectDrawArguments>());
        }
    }

    #region Shaders

    protected override NativeShader CreateShader(ShaderTranspileResult result)
    {
        uint[]? shaderIds = null;

        if (result.Vertex is not null && result.Fragment is not null)
        {
            shaderIds = new[]
            {
                compileShader(result.Vertex, GLEnum.VertexShader),
                compileShader(result.Fragment, GLEnum.FragmentShader)
            };
        }
        else if (result.Compute is not null)
        {
            shaderIds = new[]
            {
                compileShader(result.Compute, GLEnum.ComputeShader)
            };
        }
        else
        {
            throw new ArgumentException(@"Unable to determine shader type.", nameof(result));
        }

        var uniforms = new List<IUniform>();

        uint programId = api.CreateProgram();

        for (int i = 0; i < shaderIds.Length; i++)
            api.AttachShader(programId, shaderIds[i]);

        api.LinkProgram(programId);
        api.GetProgram(programId, GLEnum.LinkStatus, out int status);

        if (status == 0)
            throw new Exception($"Failed to link shader: {api.GetProgramInfoLog(programId)}");

        api.GetProgram(programId, GLEnum.ActiveUniforms, out int count);

        for (uint i = 0; i < count; i++)
        {
            api.GetActiveUniform(programId, i, 255, out _, out _, out UniformType type, out string name);

            switch (type)
            {
                case UniformType.Int:
                    uniforms.Add(new GLUniform<int>(this, programId, name, (int)i));
                    break;

                case UniformType.UnsignedInt:
                    uniforms.Add(new GLUniform<uint>(this, programId, name, (int)i));
                    break;

                case UniformType.Float:
                    uniforms.Add(new GLUniform<float>(this, programId, name, (int)i));
                    break;

                case UniformType.Double:
                    uniforms.Add(new GLUniform<double>(this, programId, name, (int)i));
                    break;

                case UniformType.FloatVec2:
                    uniforms.Add(new GLUniform<Vector2>(this, programId, name, (int)i));
                    break;

                case UniformType.FloatVec3:
                    uniforms.Add(new GLUniform<Vector3>(this, programId, name, (int)i));
                    break;

                case UniformType.FloatVec4:
                    uniforms.Add(new GLUniform<Vector4>(this, programId, name, (int)i));
                    break;

                case UniformType.FloatMat4:
                    uniforms.Add(new GLUniform<Matrix4x4>(this, programId, name, (int)i));
                    break;
            }
        }

        return new GLShader(this, programId, shaderIds, uniforms);
    }

    private uint compileShader(string code, GLEnum type)
    {
        uint id = api.CreateShader(type);
        api.ShaderSource(id, code);
        api.CompileShader(id);

        string log;

        if (!string.IsNullOrEmpty(log = api.GetShaderInfoLog(id)))
            throw new Exception($"Failed to compile shader: {log}");

        return id;
    }

    internal void UpdateShaderUniform(GLUniform uniform)
    {
        if (currentShader is null || uniform.Owner != currentShader)
            return;

        switch (uniform)
        {
            case GLUniform<int> i:
                api.Uniform1(uniform.Offset, 1, (int*)Unsafe.AsPointer(ref i.GetValueByRef()));
                break;

            case GLUniform<uint> u:
                api.Uniform1(uniform.Offset, 1, (uint*)Unsafe.AsPointer(ref u.GetValueByRef()));
                break;

            case GLUniform<float> f:
                api.Uniform1(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref f.GetValueByRef()));
                break;

            case GLUniform<double> d:
                api.Uniform1(uniform.Offset, 1, (double*)Unsafe.AsPointer(ref d.GetValueByRef()));
                break;

            case GLUniform<Vector2> v:
                api.Uniform2(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Vector3> v:
                api.Uniform3(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Vector4> v:
                api.Uniform4(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Matrix4x4> m:
                api.UniformMatrix4(uniform.Offset, 1, false, (float*)Unsafe.AsPointer(ref m.GetValueByRef()));
                break;

            default:
                throw new NotSupportedException(@"Uniform is not supported.");
        }
    }

    internal void DestroyShader(uint programId, uint[] shaderIds)
    {
        for (int i = 0; i < shaderIds.Length; i++)
        {
            api.DetachShader(programId, shaderIds[i]);
            api.DeleteShader(shaderIds[i]);
        }

        api.DeleteProgram(programId);
    }

    #endregion

    #region Buffers

    public override NativeBuffer CreateBuffer(int capacity, bool dynamic)
    {
        uint bufferId = api.GenBuffer();

        api.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        api.BufferData(BufferTargetARB.CopyWriteBuffer, (nuint)capacity, null, dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw);

        return new GLBuffer(this, bufferId, capacity, dynamic);
    }

    internal void GetBufferData(uint bufferId, nint dest, int size, int offset)
    {
        api.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        api.GetBufferSubData(BufferTargetARB.CopyWriteBuffer, offset, (nuint)size, (void*)dest);
    }

    internal void SetBufferData(uint bufferId, nint data, int size, int offset)
    {
        api.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        api.BufferSubData(BufferTargetARB.CopyWriteBuffer, offset, (nuint)size, (void*)data);
    }

    internal void DestroyBuffer(uint id) => api.DeleteBuffer(id);

    #endregion

    #region Textures

    public override NativeTexture CreateTexture(int width, int height, int depth, int levels, int layers, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, SekaiFormat format)
    {
        var target = getTextureTarget(layers, usage, type, sampleCount);
        uint textureId = api.GenTexture();

        api.BindTexture(target, textureId);
        api.TexParameterI(target, TextureParameterName.TextureMinFilter, min.ToGLEnumInt());
        api.TexParameterI(target, TextureParameterName.TextureMagFilter, mag.ToGLEnumInt());
        api.TexParameterI(target, TextureParameterName.TextureWrapS, wrapModeS.ToGLEnumInt());
        api.TexParameterI(target, TextureParameterName.TextureWrapT, wrapModeT.ToGLEnumInt());
        api.TexParameterI(target, TextureParameterName.TextureWrapR, wrapModeR.ToGLEnumInt());

        switch (target)
        {
            case TextureTarget.Texture1D:
                {
                    uint w = (uint)width;
                    for (int i = 0; i < levels; i++)
                    {
                        api.TexImage1D(TextureTarget.Texture1D, i, format.ToInternalFormat(), w, 0, format.ToPixelFormat(), format.ToPixelType(), null);
                        w = Math.Max(1, w / 2);
                    }
                    break;
                }

            case TextureTarget.Texture1DArray:
            case TextureTarget.Texture2D:
                {
                    uint w = (uint)width;
                    uint h = type == TextureType.Texture2D ? (uint)height : (uint)layers;
                    for (int i = 0; i < levels; i++)
                    {
                        api.TexImage2D(target, i, format.ToInternalFormat(), w, h, 0, format.ToPixelFormat(), format.ToPixelType(), null);
                        w = Math.Max(1, w / 2);

                        if (type == TextureType.Texture2D)
                            h = Math.Max(1, h / 2);
                    }
                    break;
                }

            case TextureTarget.Texture2DArray:
            case TextureTarget.Texture3D:
                {
                    uint w = (uint)width;
                    uint h = (uint)height;
                    uint d = type == TextureType.Texture3D ? (uint)depth : (uint)layers;
                    for (int i = 0; i < levels; i++)
                    {
                        api.TexImage3D(target, i, format.ToInternalFormat(), w, h, d, 0, format.ToPixelFormat(), format.ToPixelType(), null);
                        w = Math.Max(1, w / 2);
                        h = Math.Max(1, h / 2);

                        if (type == TextureType.Texture3D)
                            d = Math.Max(1, d / 2);
                    }
                    break;
                }

            case TextureTarget.Texture2DMultisample:
                api.TexImage2DMultisample(TextureTarget.Texture2DMultisample, getTextureSampleCount(sampleCount), format.ToInternalFormat(), (uint)width, (uint)height, false);
                break;

            case TextureTarget.Texture2DMultisampleArray:
                api.TexImage3DMultisample(TextureTarget.Texture2DMultisampleArray, getTextureSampleCount(sampleCount), format.ToInternalFormat(), (uint)width, (uint)height, (uint)layers, false);
                break;

            case TextureTarget.TextureCubeMap:
                {
                    uint w = (uint)width;
                    uint h = (uint)height;
                    for (int i = 0; i < levels; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            api.TexImage2D(TextureTarget.TextureCubeMapPositiveX + j, i, format.ToInternalFormat(), w, h, 0, format.ToPixelFormat(), format.ToPixelType(), null);
                        }

                        w = Math.Max(1, w / 2);
                        h = Math.Max(1, h / 2);
                    }
                    break;
                }

            case TextureTarget.TextureCubeMapArray:
                {
                    uint w = (uint)width;
                    uint h = (uint)height;
                    for (int i = 0; i < levels; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            api.TexImage3D(TextureTarget.TextureCubeMapPositiveX + j, i, format.ToInternalFormat(), w, h, (uint)(layers * 6), 0, format.ToPixelFormat(), format.ToPixelType(), null);
                        }

                        w = Math.Max(1, w / 2);
                        h = Math.Max(1, h / 2);
                    }
                    break;
                }
        }

        return new GLTexture(this, textureId, width, height, depth, layers, levels, min, mag, wrapModeS, wrapModeT, wrapModeR, format, type, usage, sampleCount, target);
    }

    internal void SetTextureData(uint textureId, int textureWidth, int textureHeight, int textureDepth, TextureTarget target, SekaiFormat format, nint data, int x, int y, int z, int width, int height, int depth, int layer, int level)
    {
        if (width == 0 || height == 0 || depth == 0)
            return;

        bool isCompressed = format.IsCompressed();
        int blockLength = isCompressed ? 4 : 1;
        int blockAlignedWidth = Math.Max(width, blockLength);
        int blockAlignedHeight = Math.Max(height, blockLength);

        int rowPitch = format.GetRowPitch(blockAlignedWidth);
        int depthPitch = format.GetDepthPitch(blockAlignedWidth, blockAlignedHeight);

        getTextureDimensionsAtLevel(textureWidth, textureHeight, textureDepth, level, out int w, out int h, out int d);
        width = Math.Min(width, w);
        height = Math.Min(width, h);

        int unpackAlignment = isCompressed ? 4 : format.SizeOfFormat();

        api.BindTexture(target, textureId);

        if (unpackAlignment < 4)
            api.PixelStore(PixelStoreParameter.UnpackAlignment, unpackAlignment);

        switch (target)
        {
            case TextureTarget.Texture1D when isCompressed:
                api.CompressedTexSubImage1D(target, level, x, (uint)width, format.ToInternalFormat(), (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1D:
                api.TexSubImage1D(target, level, x, (uint)width, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture1DArray when isCompressed:
                api.CompressedTexSubImage2D(target, level, x, layer, (uint)width, 1, format.ToInternalFormat(), (uint)rowPitch, (void*)data);
                break;

            case TextureTarget.Texture1DArray:
                api.TexSubImage2D(target, level, x, layer, (uint)width, 1, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture2D when isCompressed:
                api.CompressedTexSubImage2D(target, level, x, y, (uint)width, (uint)height, format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.Texture2D:
                api.TexSubImage2D(target, level, x, y, (uint)width, (uint)height, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture2DArray when isCompressed:
                api.CompressedTexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.Texture2DArray:
                api.TexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.Texture3D when isCompressed:
                api.CompressedTexSubImage3D(target, level, x, y, z, (uint)width, (uint)height, (uint)depth, format.ToInternalFormat(), (uint)(depthPitch * depth), (void*)data);
                break;

            case TextureTarget.Texture3D:
                api.TexSubImage3D(target, level, x, y, z, (uint)width, (uint)height, (uint)depth, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.TextureCubeMap when isCompressed:
                api.CompressedTexSubImage2D(getCubemapTarget(layer), level, x, y, (uint)width, (uint)height, format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMap:
                api.TexSubImage2D(getCubemapTarget(layer), level, x, y, (uint)width, (uint)height, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray when isCompressed:
                api.CompressedTexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, format.ToInternalFormat(), (uint)depthPitch, (void*)data);
                break;

            case TextureTarget.TextureCubeMapArray:
                api.TexSubImage3D(target, level, x, y, layer, (uint)width, (uint)height, 1, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
                break;

            default:
                throw new NotSupportedException();
        }

        if (unpackAlignment < 4)
            api.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
    }

    internal void GetTextureData(uint textureId, TextureTarget target, SekaiFormat format, nint data, int level)
    {
        api.BindTexture(target, textureId);
        api.GetTexImage(target, level, format.ToPixelFormat(), format.ToPixelType(), (void*)data);
    }

    internal void DestroyTexture(uint textureId) => api.DeleteTexture(textureId);

    private static TextureTarget getCubemapTarget(int layer)
    {
        switch (layer)
        {
            case 0:
                return TextureTarget.TextureCubeMapPositiveX;

            case 1:
                return TextureTarget.TextureCubeMapNegativeX;

            case 2:
                return TextureTarget.TextureCubeMapPositiveY;

            case 3:
                return TextureTarget.TextureCubeMapNegativeY;

            case 4:
                return TextureTarget.TextureCubeMapPositiveZ;

            case 5:
                return TextureTarget.TextureCubeMapNegativeZ;

            default:
                throw new InvalidOperationException(@"Unexpected layer for a cubemap texture.");
        }
    }

    private static void getTextureDimensionsAtLevel(int inWidth, int inHeight, int inDepth, int level, out int outWidth, out int outHeight, out int outDepth)
    {
        outWidth = getTextureDimension(inWidth, level);
        outDepth = getTextureDimension(inDepth, level);
        outHeight = getTextureDimension(inHeight, level);

        static int getTextureDimension(int dimension, int level)
        {
            for (int i = 0; i < level; i++)
            {
                dimension /= 2;
            }

            return Math.Max(1, dimension);
        }
    }

    private static uint getTextureSampleCount(TextureSampleCount sampleCount)
    {
        switch (sampleCount)
        {
            default:
                return 1;

            case TextureSampleCount.Count2:
                return 2;

            case TextureSampleCount.Count4:
                return 4;

            case TextureSampleCount.Count8:
                return 8;

            case TextureSampleCount.Count16:
                return 16;

            case TextureSampleCount.Count32:
                return 32;
        }
    }

    private static TextureTarget getTextureTarget(int layers, TextureUsage usage, TextureType type, TextureSampleCount sampleCount)
    {
        if (usage == TextureUsage.Cubemap && layers > 1)
            return TextureTarget.TextureCubeMapArray;

        if (usage == TextureUsage.Cubemap && layers == 1)
            return TextureTarget.TextureCubeMap;

        if (type == TextureType.Texture1D && layers > 1)
            return TextureTarget.Texture1DArray;

        if (type == TextureType.Texture1D && layers == 1)
            return TextureTarget.Texture1D;

        if (type == TextureType.Texture2D && sampleCount > TextureSampleCount.Count1 && layers > 1)
            return TextureTarget.Texture2DMultisampleArray;

        if (type == TextureType.Texture2D && sampleCount == TextureSampleCount.Count1 && layers > 1)
            return TextureTarget.Texture2DArray;

        if (type == TextureType.Texture2D && sampleCount > TextureSampleCount.Count1 && layers == 1)
            return TextureTarget.Texture2DMultisample;

        if (type == TextureType.Texture2D && sampleCount == TextureSampleCount.Count1 && layers == 1)
            return TextureTarget.Texture2D;

        if (type == TextureType.Texture3D)
            return TextureTarget.Texture3D;

        throw new NotSupportedException($@"Texture type ""{type}"" is not supported.");
    }

    #endregion

    #region Render Targets

    public override NativeRenderTarget CreateRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth)
    {
        uint frameBufferId = api.GenFramebuffer();

        api.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

        for (int i = 0; i < color.Count; i++)
        {
            var attach = color[i];
            var texture = (GLTexture)attach.Texture;

            if (texture.Layers > 1)
            {
                api.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, texture, attach.Level, attach.Layer);
            }
            else
            {
                api.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, texture.Target, texture, attach.Level);
            }
        }

        if (depth.HasValue)
        {
            var texture = (GLTexture)depth.Value.Texture;

            if (texture.Layers > 1)
            {
                api.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, texture, depth.Value.Level, depth.Value.Layer);
            }
            else
            {
                api.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, texture.Target, texture, depth.Value.Level);
            }
        }

        if (api.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
            throw new InvalidOperationException(@"Framebuffer is not ready to be bound.");

        return new GLRenderTarget(this, frameBufferId, color, depth);
    }

    internal void DestroyRenderTarget(uint framebufferId) => api.DeleteFramebuffer(framebufferId);

    #endregion
}

public class GLException : Exception
{
    public GLException(string? message)
        : base(message)
    {
    }
}
