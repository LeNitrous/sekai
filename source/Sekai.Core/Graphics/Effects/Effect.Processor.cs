// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Veldrid;

namespace Sekai.Graphics.Effects;

public partial class Effect
{
    private static Shader[] process(GraphicsDevice device, string code, out EffectKind kind, out Reflection reflect)
    {
        var result = transpile(code);

        if (!string.IsNullOrEmpty(result.Vertex) && !string.IsNullOrEmpty(result.Fragment))
        {
            kind = EffectKind.Graphic;

            return new Shader[]
            {
                device.Factory.CreateShader(new(ShaderStages.Vertex, compile(device, Stage.Vertex, result.Vertex, out reflect), "main")),
                device.Factory.CreateShader(new(ShaderStages.Fragment, compile(device, Stage.Fragment, result.Fragment, out _), "main")),
            };
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static ShaderTranspileResult transpile(string code)
    {
        var builder = new StringBuilder();
        builder.AppendLine(header);

        var mat = new StringBuilder();
        var res = new StringBuilder();
        int resId = 1;

        foreach (var e in getExterns(code))
        {
            if (e.Type.Contains("texture"))
            {
                builder.Append("#define ").Append(e.Name).Append(" sampler2D(g_internal_TEXTURE_").Append(e.Name).Append(", g_internal_SAMPLER_").Append(e.Name).AppendLine(")");
                res.Append("layout (set = 1, binding =").Append(resId).Append(") uniform ").Append(e.Type).Append(" g_internal_TEXTURE_").Append(e.Name).AppendLine(";");
                res.Append("layout (set = 1, binding =").Append(resId + 1).Append(") uniform ").Append(getSamplerType(e.Type)).Append(" g_internal_SAMPLER_").Append(e.Name).AppendLine(";");
                resId += 2;
            }
            else
            {
                mat.Append(e.Type).Append(' ').Append(e.Identifier).AppendLine(";");
            }

            code = code.Replace(e.Value, string.Empty);
        }

        builder.AppendLine();
        builder.Append("layout (set = 1, binding = 0) uniform ").AppendLine(buffer_user);
        builder.AppendLine("{");
        builder.Append(mat);
        builder.AppendLine("};");
        builder.AppendLine();
        builder.Append(res);
        builder.AppendLine();

        var methods = getMethods(code);
        var attribs = getAttribs(code);

        var result = new ShaderTranspileResult();

        foreach (var method in methods)
        {
            switch (method.Stage)
            {
                case Stage.Vertex:
                    result.Vertex = createVertexCode(builder, code, methods, attribs);
                    break;

                case Stage.Fragment:
                    result.Fragment = createFragmentCode(builder, code, methods, attribs);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        return result;
    }

    private static string createVertexCode(StringBuilder header, string code, IReadOnlyList<MethodToken> methods, IReadOnlyList<AttribToken> attribs)
    {
        var main = new StringBuilder();
        main.Append(header);
        main.AppendLine();

        var attr = new StringBuilder();

        for (int i = 0; i < attribs.Count; i++)
        {
            var attrib = attribs[i];

            main.Append("layout(location = ").Append(i).Append(") in ").Append(attrib.Type).Append(' ').Append(attrib.Name).AppendLine(";");
            main.Append("layout(location = ").Append(i).Append(") out ").Append(attrib.Type).Append(" v_internal_").Append(attrib.Name).AppendLine(";");

            attr.Append("v_internal_").Append(attrib.Name).Append(" = ").Append(attrib.Name).AppendLine(";");
            code = code.Replace(attrib.Value, string.Empty);
        }

        foreach (var method in methods)
        {
            if (method.Stage == Stage.Vertex)
            {
                continue;
            }

            code = code.Replace(method.Value, string.Empty);
        }

        main.AppendLine();
        main.AppendLine(code);
        main.AppendLine();
        main.AppendLine("void main() {");
        main.AppendLine("vert();");
        main.Append(attr);
        main.AppendLine("}");

        return main.ToString();
    }

    private static string createFragmentCode(StringBuilder header, string code, IReadOnlyList<MethodToken> methods, IReadOnlyList<AttribToken> attribs)
    {
        var main = new StringBuilder();
        main.Append(header);
        main.AppendLine();

        for (int i = 0; i < attribs.Count; i++)
        {
            var attrib = attribs[i];

            main.Append("layout (location = ").Append(i).Append(") in ").Append(attrib.Type).Append(' ').Append(attrib.Name).AppendLine(";");
            code = code.Replace(attrib.Value, string.Empty);
        }

        foreach (var method in methods)
        {
            if (method.Stage == Stage.Fragment)
            {
                var outputs = getOutputs(method.Value);

                for (int i = 0; i < outputs.Count; i++)
                {
                    main.Append("layout (location = ").Append(i).Append(") out vec4 SK_COLOR").Append(i).AppendLine(";");
                }
            }
            else
            {
                code = code.Replace(method.Value, string.Empty);
            }
        }

        main.Append(code);
        main.AppendLine();
        main.AppendLine("void main() {");
        main.AppendLine("frag();");
        main.AppendLine("}");

        return main.ToString();
    }

    private static IReadOnlyList<MethodToken> getMethods(string code)
        => getTokens(code, regex_method, match => getMethod(code, match));

    private static IReadOnlyList<ExternToken> getExterns(string code)
        => getTokens(code, regex_extern, match => new ExternToken(match.Groups["Type"].Value, match.Groups["Name"].Value, match.Groups["Identifier"].Value, code[match.Index..(match.Index + match.Length)]));

    private static IReadOnlyList<AttribToken> getAttribs(string code)
        => getTokens(code, regex_attrib, match => new AttribToken(match.Groups["Type"].Value, match.Groups["Name"].Value, code[match.Index..(match.Index + match.Length)]));

    private static IReadOnlyList<Token> getOutputs(string code)
        => getTokens(code, regex_output, match => new Token(code[match.Index..(match.Index + match.Length)]));

    private static MethodToken getMethod(string code, Match match)
    {
        int current = match.Index;

        while (code[current] != '{')
        {
            current++;
        }

        int depth = 0;

        do
        {
            if (current > code.Length)
                throw new ArgumentException("Invalid shader method.", nameof(code));

            if (code[current] == '{')
            {
                depth++;
            }

            if (code[current] == '}')
            {
                depth--;
            }

            current++;
        }
        while (depth != 0);

        return new(stageMapping[match.Groups["Name"].Value], code[match.Index..current]);
    }

    private static IReadOnlyList<T> getTokens<T>(string code, Regex regex, Func<Match, T> creator)
        where T : Token
    {
        var match = regex.Match(code);
        var matches = new List<T>();

        do
        {
            if (!match.Success)
                break;

            matches.Add(creator(match));
        }
        while ((match = match.NextMatch()).Success);

        return matches;
    }

    private static string getSamplerType(string type)
    {
        if (type.Contains("shadow", StringComparison.InvariantCultureIgnoreCase))
        {
            return "samplerShadow";
        }

        return "sampler";
    }

    private static readonly Dictionary<string, Stage> stageMapping = new()
    {
        { "vert", Stage.Vertex },
        { "frag", Stage.Fragment }
    };

    private static readonly Regex regex_attrib = regex_attrib_generator();
    private static readonly Regex regex_extern = regex_extern_generator();
    private static readonly Regex regex_method = regex_method_generator();
    private static readonly Regex regex_output = regex_output_generator();

    [GeneratedRegex(@"(?:attrib)\s+(?<Type>\w+)\s+(?<Name>\w+);", RegexOptions.Compiled)]
    private static partial Regex regex_attrib_generator();

    [GeneratedRegex(@"(?:void)\s*(?<Name>vert|frag)\s*\(\)", RegexOptions.Compiled)]
    private static partial Regex regex_method_generator();

    [GeneratedRegex(@"(?:uniform|extern)\s+(?<Type>\w+)\s+(?<Identifier>(?<Name>\w+)(?>\[\d+\])?);", RegexOptions.Compiled)]
    private static partial Regex regex_extern_generator();

    [GeneratedRegex(@"SK_COLOR[0-7]", RegexOptions.Compiled)]
    private static partial Regex regex_output_generator();

    private const string buffer_user = "g_internal_USER_PROPERTIES";

    private const string header = @"
#version 460

#define SK_POSITION gl_Position
#define SK_POINT_SIZE gl_PointSize
#define SK_CLIP_DISTANCE gl_ClipDistance
#define SK_DRAW_ID gl_DrawID
#define SK_VERTEX_ID gl_VertexID
#define SK_VERTEX_BASE gl_BaseVertex
#define SK_INSTANCE_ID gl_InstanceID
#define SK_INSTANCE_BASE gl_BaseInstance

layout (set = 0, binding = 0) uniform g_internal_PROJECTION
{
    mat4 g_internal_ProjMatrix;
    mat4 g_internal_ViewMatrix;
    mat4 g_internal_ModelMatrix;
};

#define P_MATRIX g_internal_ProjMatrix
#define V_MATRIX g_internal_ViewMatrix
#define M_MATRIX g_internal_ModelMatrix
#define OBJECT_TO_VIEW g_internal_ViewMatrix * g_internal_ModelMatrix
#define OBJECT_TO_CLIP g_internal_ProjMatrix * g_internal_ViewMatrix * g_internal_ModelMatrix
";

    private class Token
    {
        public string Value { get; }

        public Token(string value)
        {
            Value = value;
        }
    }

    private class MethodToken : Token
    {
        public Stage Stage { get; }

        public MethodToken(Stage stage, string value)
            : base(value)
        {
            Stage = stage;
        }
    }

    private class ExternToken : Token
    {
        public string Type { get; }
        public string Name { get; }
        public string Identifier { get; }

        public ExternToken(string type, string name, string identifier, string value)
            : base(value)
        {
            Type = type;
            Name = name;
            Identifier = identifier;
        }
    }

    private class AttribToken : Token
    {
        public string Type { get; }
        public string Name { get; }

        public AttribToken(string type, string name, string value)
            : base(value)
        {
            Type = type;
            Name = name;
        }
    }

    private class ShaderTranspileResult
    {
        public string? Vertex { get; set; }

        public string? Fragment { get; set; }
    }
}
