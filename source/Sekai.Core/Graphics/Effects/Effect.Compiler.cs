// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Spirzza.Interop.Shaderc;
using Spirzza.Interop.SpirvCross;
using static Spirzza.Interop.Shaderc.Shaderc;
using static Spirzza.Interop.SpirvCross.SpirvCross;

namespace Sekai.Graphics.Effects;

public partial class Effect
{
    private static unsafe byte[] compile(Stage stage, string code, out Reflection reflect)
    {
        var comp = shaderc_compiler_initialize();
        var opts = shaderc_compile_options_initialize();

        shaderc_compile_options_set_source_language(opts, shaderc_source_language.shaderc_source_language_glsl);

        var kind = stage switch
        {
            Stage.Vertex => shaderc_shader_kind.shaderc_vertex_shader,
            Stage.Fragment => shaderc_shader_kind.shaderc_fragment_shader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage)),
        };

        Span<byte> source = stackalloc byte[Encoding.ASCII.GetByteCount(code)];
        Encoding.ASCII.GetBytes(code.AsSpan(), source);

        shaderc_compilation_result* result;

        string? error = null;
        byte[]? bytes = null;

        fixed (byte* sourceHandle = source)
        fixed (byte* nameHandle = &name[0])
        fixed (byte* entryHandle = &entry[0])
        {
            result = shaderc_compile_into_spv(comp, (sbyte*)sourceHandle, (nuint)source.Length, kind, (sbyte*)nameHandle, (sbyte*)entryHandle, opts);
        }

        if (shaderc_result_get_compilation_status(result) != shaderc_compilation_status.shaderc_compilation_status_success)
        {
            error = Marshal.PtrToStringAnsi((nint)shaderc_result_get_error_message(result));
        }
        else
        {
            bytes = new byte[(int)shaderc_result_get_length(result)];

            fixed (byte* bytesHandle = bytes)
            {
                Unsafe.CopyBlock(bytesHandle, shaderc_result_get_bytes(result), (uint)bytes.Length);
            }
        }

        shaderc_result_release(result);
        shaderc_compiler_release(comp);
        shaderc_compile_options_release(opts);

        if (!string.IsNullOrEmpty(error))
        {
            throw new EffectCompilationException(error);
        }

        if (bytes is null)
        {
            throw new InvalidOperationException();
        }

        reflect = JsonSerializer.Deserialize<Reflection>(compile(spvc_backend.SPVC_BACKEND_JSON, bytes))!;
        return bytes;
    }

    private static unsafe byte[] compile(spvc_backend backend, Span<byte> data)
    {
        byte[]? bytes = null;
        string? error = null;

        spvc_context* context;
        spvc_context_create(&context);

        spvc_parsed_ir* ir;
        spvc_result result;

        fixed (byte* dataHandle = data)
        {
            result = spvc_context_parse_spirv(context, (SpvId*)dataHandle, (nuint)(data.Length / Unsafe.SizeOf<SpvId>()), & ir);
        }

        if (result != spvc_result.SPVC_SUCCESS)
        {
            error = Marshal.PtrToStringAnsi((nint)spvc_context_get_last_error_string(context));
        }
        else
        {
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
            }

            spvc_compiler_install_compiler_options(comp, opts);

            if (backend == spvc_backend.SPVC_BACKEND_HLSL)
            {
                spvc_compiler_build_combined_image_samplers(comp);
            }

            sbyte* compiled;
            result = spvc_compiler_compile(comp, &compiled);

            if (result != spvc_result.SPVC_SUCCESS)
            {
                error = Marshal.PtrToStringAnsi((nint)spvc_context_get_last_error_string(context));
            }
            else
            {
                bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)compiled).ToArray();
            }
        }

        spvc_context_destroy(context);

        if (!string.IsNullOrEmpty(error))
        {
            throw new EffectCompilationException(error);
        }

        if (bytes is null)
        {
            // We don't know what caused this to be null when it shouldn't!
            throw new InvalidOperationException();
        }

        return bytes;
    }

    private static unsafe byte[] compile(GraphicsDevice device, Stage stage, string code, out Reflection reflect)
    {
        if (device.API == GraphicsAPI.Vulkan)
        {
            return compile(stage, code, out reflect);
        }

        var target = device.API switch
        {
            GraphicsAPI.D3D11 => spvc_backend.SPVC_BACKEND_HLSL,
            GraphicsAPI.Metal => spvc_backend.SPVC_BACKEND_MSL,
            GraphicsAPI.OpenGL => spvc_backend.SPVC_BACKEND_GLSL,
            _ => throw new NotSupportedException(),
        };

        return compile(target, compile(stage, code, out reflect).AsSpan());
    }

    private enum Stage
    {
        Vertex,
        Fragment,
    }

    private static readonly byte[] name = Encoding.ASCII.GetBytes("internal");
    private static readonly byte[] entry = Encoding.ASCII.GetBytes("main");
}
