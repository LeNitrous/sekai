// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Spirzza.Interop.Shaderc;
using Spirzza.Interop.SpirvCross;
using static Spirzza.Interop.Shaderc.Shaderc;
using static Spirzza.Interop.SpirvCross.SpirvCross;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Shader code that can be used as a source for <see cref="Shader"/>s.
/// </summary>
public readonly struct ShaderCode : IEquatable<ShaderCode>
{
    /// <summary>
    /// The shader stage this code is associated with.
    /// </summary>
    public ShaderStage Stage { get; }

    private readonly byte[] bytes;
    private readonly ShaderConstant[] constants;

    private ShaderCode(ShaderStage stage, byte[] bytes, ShaderConstant[] constants)
    {
        this.Stage = stage;
        this.bytes = bytes;
        this.constants = constants;
    }

    /// <summary>
    /// Gets reflection metadata on this <see cref="ShaderCode"/>.
    /// </summary>
    /// <returns>Shader reflection metadata.</returns>
    public ShaderReflection Reflect()
    {
        return JsonSerializer.Deserialize(transpile(bytes, spvc_backend.SPVC_BACKEND_JSON)!, ShaderReflectionJsonContext.Default.ShaderReflection)!;
    }

    /// <summary>
    /// Gets the shader code as text.
    /// </summary>
    /// <param name="language">The shader language to transpile as.</param>
    /// <returns>Shader code as text.</returns>
    /// <remarks>
    /// As <see cref="ShaderCode"/> contains intermediary representation of shader code, it will not be able to translate back the source text it originated from.
    /// </remarks>
    public string? GetText(ShaderLanguage language)
    {
        var backend = language switch
        {
            ShaderLanguage.GLSL => spvc_backend.SPVC_BACKEND_GLSL,
            ShaderLanguage.HLSL => spvc_backend.SPVC_BACKEND_HLSL,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };

        return transpile(bytes, backend);
    }

    /// <summary>
    /// Copies the contents of the shader code to a buffer.
    /// </summary>
    /// <param name="buffer">The destination buffer to copy to.</param>
    public void GetBytes(Span<byte> buffer)
    {
        bytes.CopyTo(buffer);
    }

    /// <inheritdoc cref="GetBytes(Span{byte})"/>
    public void GetBytes(byte[] buffer)
    {
        GetBytes(buffer.AsSpan());
    }

    private static unsafe string? transpile(byte[] source, spvc_backend backend)
    {
        spvc_context* context;
        spvc_context_create(&context);

        spvc_parsed_ir* ir;
        spvc_result result;

        fixed (byte* bytes = source)
        {
            result = spvc_context_parse_spirv(context, (SpvId*)bytes, (nuint)(source.Length / Unsafe.SizeOf<SpvId>()), &ir);
        }

        if (result != spvc_result.SPVC_SUCCESS)
        {
            string? error = Marshal.PtrToStringAnsi((nint)spvc_context_get_last_error_string(context));
            spvc_context_destroy(context);

            throw new ShaderCompilationException(error);
        }

        spvc_compiler* comp;
        spvc_context_create_compiler(context, backend, ir, spvc_capture_mode.SPVC_CAPTURE_MODE_COPY, &comp);

        spvc_compiler_options* opts;
        spvc_compiler_create_compiler_options(comp, &opts);

        switch (backend)
        {
            case spvc_backend.SPVC_BACKEND_GLSL:
                spvc_compiler_options_set_uint(opts, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 430);
                spvc_compiler_options_set_bool(opts, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, SPVC_FALSE);
                break;

            case spvc_backend.SPVC_BACKEND_HLSL:
                spvc_compiler_options_set_uint(opts, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL, 50);
                spvc_compiler_options_set_bool(opts, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_FLATTEN_MATRIX_VERTEX_INPUT_SEMANTICS, SPVC_TRUE);
                break;

            case spvc_backend.SPVC_BACKEND_JSON:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }

        spvc_compiler_install_compiler_options(comp, opts);

        spvc_variable_id id;
        spvc_compiler_build_dummy_sampler_for_combined_images(comp, &id);
        spvc_compiler_build_combined_image_samplers(comp);

        nuint samplerCount;
        spvc_combined_image_sampler* samplers;
        spvc_compiler_get_combined_image_samplers(comp, &samplers, &samplerCount);

        for (int i = 0; i < (int)samplerCount; i++)
        {
            uint decoration = spvc_compiler_get_decoration(comp, samplers[i].image_id.Value, SpvDecoration.SpvDecorationBinding);
            spvc_compiler_set_decoration(comp, samplers[i].combined_id.Value, SpvDecoration.SpvDecorationBinding, decoration);
        }

        sbyte* compiled;
        result = spvc_compiler_compile(comp, &compiled);

        if (result != spvc_result.SPVC_SUCCESS)
        {
            string? error = Marshal.PtrToStringAnsi((nint)spvc_context_get_last_error_string(context));
            spvc_context_destroy(context);
            throw new ShaderCompilationException(error);
        }

        string? sCompiled = Marshal.PtrToStringAnsi((nint)compiled);
        spvc_context_destroy(context);

        return sCompiled;
    }

    /// <summary>
    /// Creates a new <see cref="ShaderCode"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream that contains shader code.</param>
    /// <param name="language">The language the shader code is written in.</param>
    /// <param name="encoding">The encoding of the stream.</param>
    /// <returns>A new shader code.</returns>
    public static ShaderCode From(Stream stream, ShaderStage stage, ShaderLanguage language = ShaderLanguage.GLSL, Encoding? encoding = null, params ShaderConstant[] constants)
    {
        using var reader = new StreamReader(stream, encoding ??= Encoding.UTF8);
        return From(reader.ReadToEnd(), stage, language, encoding, constants);
    }

    /// <summary>
    /// Creates a new <see cref="ShaderCode"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="text">The shader code as text.</param>
    /// <param name="stage">The shader code's stage.</param>
    /// <param name="language">The language the shader code is written in.</param>
    /// <param name="encoding">The encoding of the text.</param>
    /// <returns>A new shader code.</returns>
    public static ShaderCode From(string text, ShaderStage stage, ShaderLanguage language = ShaderLanguage.GLSL, Encoding? encoding = null, params ShaderConstant[] constants)
    {
        return From(text.AsSpan(), stage, language, encoding, constants);
    }

    /// <summary>
    /// Creates a new <see cref="ShaderCode"/> from a <see cref="ReadOnlySpan{char}"/>.
    /// </summary>
    /// <param name="text">The shader code as text.</param>
    /// <param name="stage">The shader code's stage.</param>
    /// <param name="language">The language the shader code is written in.</param>
    /// <param name="encoding">The encoding of the text.</param>
    /// <returns>A new shader code.</returns>
    public static ShaderCode From(ReadOnlySpan<char> text, ShaderStage stage, ShaderLanguage language = ShaderLanguage.GLSL, Encoding? encoding = null, params ShaderConstant[] constants)
    {
        Span<byte> buffer = stackalloc byte[(encoding ??= Encoding.UTF8).GetByteCount(text)];

        if (encoding.GetBytes(text, buffer) <= 0)
        {
            throw new ShaderCompilationException("Failed to encode shader code.");
        }

        return From(buffer, stage, language, constants);
    }

    /// <summary>
    /// Creates a new <see cref="ShaderCode"/> from a <see cref="byte[]"/>.
    /// </summary>
    /// <param name="bytes">The shader code as a byte array.</param>
    /// <param name="stage">The shader code's stage.</param>
    /// <param name="language">The language the shader code is written in.</param>
    /// <returns>A new shader code.</returns>
    public static ShaderCode From(byte[] bytes, ShaderStage stage, ShaderLanguage language = ShaderLanguage.GLSL, params ShaderConstant[] constants)
    {
        return From(bytes.AsSpan(), stage, language, constants);
    }

    /// <summary>
    /// Creates a new <see cref="ShaderCode"/> from a <see cref="ReadOnlySpan{byte}"/>.
    /// </summary>
    /// <param name="bytes">The shader code as a byte array.</param>
    /// <param name="stage">The shader code's stage.</param>
    /// <param name="language">The language the shader code is written in.</param>
    /// <returns>A new shader code.</returns>
    public static unsafe ShaderCode From(ReadOnlySpan<byte> bytes, ShaderStage stage, ShaderLanguage language = ShaderLanguage.GLSL, params ShaderConstant[] constants)
    {
        var comp = shaderc_compiler_initialize();
        var opts = shaderc_compile_options_initialize();
        var lang = language switch
        {
            ShaderLanguage.GLSL => shaderc_source_language.shaderc_source_language_glsl,
            ShaderLanguage.HLSL => shaderc_source_language.shaderc_source_language_hlsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };

        shaderc_compile_options_set_source_language(opts, lang);
        shaderc_compile_options_set_auto_combined_image_sampler(opts, 1);

        var kind = stage switch
        {
            ShaderStage.Vertex => shaderc_shader_kind.shaderc_vertex_shader,
            ShaderStage.Geometry => shaderc_shader_kind.shaderc_geometry_shader,
            ShaderStage.Fragment => shaderc_shader_kind.shaderc_fragment_shader,
            ShaderStage.TesselationControl => shaderc_shader_kind.shaderc_tess_control_shader,
            ShaderStage.TesselationEvaluation => shaderc_shader_kind.shaderc_tess_evaluation_shader,
            ShaderStage.Compute => shaderc_shader_kind.shaderc_compute_shader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null),
        };

        shaderc_compilation_result* result;

        fixed (byte* source = bytes)
        fixed (byte* file = ShaderCode.file)
        fixed (byte* main = ShaderCode.main)
        {
            result = shaderc_compile_into_spv(comp, (sbyte*)source, (nuint)bytes.Length, kind, (sbyte*)file, (sbyte*)main, opts);
        }

        if (shaderc_result_get_compilation_status(result) != shaderc_compilation_status.shaderc_compilation_status_success)
        {
            string? message = Marshal.PtrToStringAnsi((nint)shaderc_result_get_error_message(result));

            shaderc_result_release(result);
            shaderc_compiler_release(comp);
            shaderc_compile_options_release(opts);
            throw new ShaderCompilationException($"Failed to compile shader.\n{message}");
        }

        byte[] buffer = new byte[(int)shaderc_result_get_length(result)];

        fixed (byte* bufferHandle = buffer)
        {
            Unsafe.CopyBlock(bufferHandle, shaderc_result_get_bytes(result), (uint)buffer.Length);
        }

        shaderc_result_release(result);
        shaderc_compiler_release(comp);
        shaderc_compile_options_release(opts);

        return new(stage, buffer, constants);
    }

    private static readonly byte[] main = "main"u8.ToArray();
    private static readonly byte[] file = "shader"u8.ToArray();

    public bool Equals(ShaderCode other)
    {
        return bytes.SequenceEqual(other.bytes);
    }

    public override bool Equals(object? obj)
    {
        return obj is ShaderCode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Stage, bytes);
    }

    public static bool operator ==(ShaderCode left, ShaderCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ShaderCode left, ShaderCode right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Exception thrown when a shader has failed to compile.
/// </summary>
public class ShaderCompilationException : Exception
{
    public ShaderCompilationException()
    {
    }

    public ShaderCompilationException(string? message)
        : base(message)
    {
    }

    public ShaderCompilationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
