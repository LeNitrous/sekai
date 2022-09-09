// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Compiler;

public static class EffectAnalyzer
{
    public static EffectAnalysisResult Analyze(EffectDocumentPass pass)
    {
        string[] knownTypes = types_builtin.Concat(pass.Structs.Select(s => s.Name)).ToArray();

        foreach (var @struct in pass.Structs)
        {
            foreach (var member in @struct.Members)
            {
                if (!knownTypes.Contains(member.Type))
                    throw new Exception($"Unknown type \"{member.Type}\" of {member.Name} in {@struct.Name}.");

                if (types_opaque.Contains(member.Type))
                    throw new Exception(@"Opaque types cannot be members in structs.");
            }
        }

        foreach (var member in pass.Members)
        {
            if (!knownTypes.Contains(member.Type))
                throw new Exception($"Unknown type \"{member.Type}\" of {member.Name}.");

            if (!qualifiers.Contains(member.Qualifier))
                throw new Exception($"Unknown qualifier \"{member.Qualifier}\" of {member.Name}.");
        }

        foreach (var method in pass.Methods)
        {
            if (!knownTypes.Contains(method.Type))
                throw new Exception($"Unknown return type \"{method.Type}\" of {method.Name}.");
        }

        var baseAlignments = new Dictionary<string, int>(base_alignments);

        foreach (var @struct in pass.Structs)
        {
            if (baseAlignments.ContainsKey(@struct.Name))
                continue;

            baseAlignments.Add(@struct.Name, calculateAlignment(@struct, pass.Structs, baseAlignments));
        }

        var paramInfos = new List<EffectParameterInfo>();
        var paramMemberInfos = pass.Members.Where(m => m.Qualifier != "stage").ToArray();

        for (int i = 0; i < paramMemberInfos.Length; i++)
        {
            var info = paramMemberInfos[i];
            int size = 0;

            if (int.TryParse(info.Size, out int arrayLength) && arrayLength > 0)
            {
                if (types_scalar.Contains(info.Type))
                {
                    size = arrayLength * 16;
                }
                else
                {
                    size = arrayLength * baseAlignments[info.Type];
                }
            }
            else if (!types_opaque.Contains(info.Type))
            {
                size = baseAlignments[info.Type];
            }

            var flags = EffectParameterFlags.None;

            if (info.Qualifier == "buffer")
                flags |= EffectParameterFlags.Buffer;

            if (info.Qualifier == "uniform")
                flags |= EffectParameterFlags.Uniform;

            if (types_opaque_image.Contains(info.Type))
                flags |= EffectParameterFlags.Image;

            if (types_opaque_texture.Contains(info.Type))
                flags |= EffectParameterFlags.Texture;

            if (types_opaque_sampler.Contains(info.Type))
                flags |= EffectParameterFlags.Sampler;

            if (types_opaque_cubemap.Contains(info.Type))
                flags |= EffectParameterFlags.Cubemap;

            if (info.Type.Contains("1D"))
                flags |= EffectParameterFlags.Texture1D;

            if (info.Type.Contains("2D"))
                flags |= EffectParameterFlags.Texture2D;

            if (info.Type.Contains("3D"))
                flags |= EffectParameterFlags.Texture3D;

            paramInfos.Add(new EffectParameterInfo(info.Name, info.Type, size, flags));
        }

        var stageInfos = pass.Members
            .Except(paramMemberInfos)
            .Select(e => new EffectStageInfo(e.Name, e.Type))
            .ToArray();

        return new EffectAnalysisResult(stageInfos, paramInfos.ToArray());
    }

    private static int calculateAlignment(EffectStructInfo @struct, IEnumerable<EffectStructInfo> structs, Dictionary<string, int> sizes)
    {
        int alignment = 0;

        foreach (var member in @struct.Members)
        {
            if (!sizes.TryGetValue(member.Type, out int size))
            {
                size = calculateAlignment(structs.Single(s => s.Name == member.Type), structs, sizes);
                sizes.Add(member.Type, size);
            }

            alignment += size;
        }

        return alignment += 16 - (alignment % 16);
    }

    private static string[] qualifiers { get; } = new[]
    {
        "stage",
        "buffer",
        "uniform",
    };

    private static Dictionary<string, int> base_alignments { get; } = new()
    {
        { "int", 4 },
        { "bool", 4 },
        { "uint", 4 },
        { "float", 4 },
        { "double", 4 },
        { "vec2", 8 },
        { "vec3", 16 },
        { "vec4", 16 },
        { "mat4", 64 },
        { "mat3x2", 48 },
        { "mat4x4", 64 },
    };

    private static string[] types_opaque_image { get; } = new[]
    {
        "image1D",
        "image2D",
        "image3D",
        "image2DMS",
        "image2DRect",
        "image2DArray",
        "imageCube",
        "imageCubeArray",
        "imageBuffer",
    };

    private static string[] types_opaque_sampler { get; } = new[]
    {
        "sampler",
        "samplerShadow",
    };

    private static string[] types_opaque_texture { get; } = new[]
    {
        "texture1D",
        "texture2D",
        "texture3D",
        "texture1DArray",
        "texture2DArray",
        "texture2DMS",
        "texture2DRect",
        "texture2DMSArray",
        "textureCube",
        "textureCubeArray",
        "textureBuffer",
    };

    private static string[] types_opaque_cubemap { get; } = new[]
    {
        "imageCube",
        "imageCubeArray",
        "textureCube",
        "textureCubeArray",
    };

    private static string[] types_scalar { get; } = new[]
    {
        "int",
        "bool",
        "uint",
        "float",
        "double",
    };

    private static string[] types_structs_builtin { get; } = new[]
    {
        "vec2",
        "vec3",
        "vec4",
        "mat4",
        "mat3x2",
        "mat4x4",
    };

    private static string[] types_opaque { get; } = types_opaque_image.Concat(types_opaque_sampler).Concat(types_opaque_texture).ToArray();
    private static string[] types_builtin { get; } = types_structs_builtin.Concat(types_opaque).Concat(types_scalar).ToArray();
}
