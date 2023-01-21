// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sekai.Extensions;

namespace Sekai.Graphics.Shaders;

public abstract partial class ShaderTranspiler : FrameworkObject
{
    protected abstract string Header { get; }

    public ShaderTranspileResult Transpile(string code)
    {
        var result = new ShaderTranspileResult();
        var attribs = getAttribs(code);
        var externs = getExterns(code);
        var methods = getMethods(code);

        foreach (var method in methods)
        {
            switch (method.Target)
            {
                case ShaderStage.Vertex:
                    result.Vertex = createVertexCode(code, methods, attribs, externs);
                    break;

                case ShaderStage.Fragment:
                    result.Fragment = createFragmentCode(code, methods, attribs, externs);
                    break;

                case ShaderStage.Compute:
                    result.Compute = createComputeCode(code, methods, attribs, externs);
                    break;
            }
        }

        return result;
    }

    protected virtual string FormatUniform(string type, string name, ShaderStage? stage = null) => $"uniform {type} {name};";

    private string createVertexCode(string code, IReadOnlyList<ShaderMethod> methods, IReadOnlyList<ShaderAttribute> attribs, IReadOnlyList<ShaderUniform> externs)
    {
        var main = new StringBuilder();
        var body = new StringBuilder();

        for (int i = 0; i < attribs.Count; i++)
        {
            string a = $"layout(location = {i}) in {attribs[i].Type} {attribs[i].Name};";
            string b = $"layout(location = {i}) out {attribs[i].Type} v_internal_{attribs[i].Name};";

            body.AppendLine($"v_internal_{attribs[i].Name} = {attribs[i].Name};");
            code = code.Replace(attribs[i].Value.ToString(), string.Join(Environment.NewLine, a, b));
        }

        foreach (var ext in externs)
            code = code.Replace(ext.Value.ToString(), FormatUniform(ext.Type, ext.Name, ShaderStage.Vertex));

        foreach (var method in methods)
        {
            if (method.Target == ShaderStage.Vertex)
                continue;

            code = code.Replace(method.Value.ToString(), string.Empty);
        }

        main.AppendLine(Header);
        appendShaderBuiltins(main);

        main.AppendLine(defines);
        main.AppendLine(code);
        main.AppendLine(@"void main() {");
        main.Append(body);
        main.AppendLine(@"vert();");
        main.AppendLine(@"}");

        return main.ToString();
    }

    private string createFragmentCode(string code, IReadOnlyList<ShaderMethod> methods, IReadOnlyList<ShaderAttribute> attribs, IReadOnlyList<ShaderUniform> externs)
    {
        var main = new StringBuilder();
        var head = new StringBuilder();

        for (int i = 0; i < attribs.Count; i++)
            code = code.Replace(attribs[i].Value.ToString(), string.Join(Environment.NewLine, $"layout(location = {i}) in {attribs[i].Type} {attribs[i].Name};"));

        foreach (var ext in externs)
            code = code.Replace(ext.Value.ToString(), FormatUniform(ext.Type, ext.Name, ShaderStage.Fragment));

        foreach (var method in methods)
        {
            if (method.Target == ShaderStage.Fragment)
            {
                var outputs = getOutputs(method.Value.ToString());

                for (int i = 0; i < outputs.Count; i++)
                    head.AppendLine($"layout(location = {i}) out vec4 SK_COLOR{i};");
            }
            else
            {
                code = code.Replace(method.Value.ToString(), string.Empty);
            }
        }

        main.AppendLine(Header);
        appendShaderBuiltins(main);

        main.AppendLine(defines);
        main.Append(head);
        main.AppendLine(code);
        main.AppendLine(@"void main() {");
        main.AppendLine(@"frag();");
        main.AppendLine(@"}");

        return main.ToString();
    }

    private string createComputeCode(string code, IReadOnlyList<ShaderMethod> methods, IReadOnlyList<ShaderAttribute> attribs, IReadOnlyList<ShaderUniform> externs)
    {
        var main = new StringBuilder();

        for (int i = 0; i < attribs.Count; i++)
            code = code.Replace(attribs[i].Value.ToString(), string.Join(Environment.NewLine, $"layout(location = {i}) in {attribs[i].Type} {attribs[i].Name};"));

        foreach (var ext in externs)
            code = code.Replace(ext.Value.ToString(), FormatUniform(ext.Type, ext.Name, ShaderStage.Compute));

        foreach (var method in methods)
        {
            if (method.Target == ShaderStage.Compute)
                continue;

            code = code.Replace(method.Value.ToString(), string.Empty);
        }

        main.AppendLine(Header);
        appendShaderBuiltins(main);

        main.AppendLine(defines);
        main.AppendLine(code);
        main.AppendLine(@"void main() {");
        main.AppendLine(@"comp();");
        main.AppendLine(@"}");

        return main.ToString();
    }

    private static IReadOnlyList<ShaderAttribute> getAttribs(string code)
        => retrieveToken(code, regex_attrib, match => new ShaderAttribute(code, match.Index, match.Length, match.Groups["Type"].Value, match.Groups["Name"].Value));

    private static IReadOnlyList<ShaderUniform> getExterns(string code)
        => retrieveToken(code, regex_extern, match => new ShaderUniform(code, match.Index, match.Length, match.Groups["Type"].Value, match.Groups["Name"].Value));

    private static IReadOnlyList<ShaderOutput> getOutputs(string code)
        => retrieveToken(code, regex_output, match => new ShaderOutput(code, match.Index, match.Length));

    private static IReadOnlyList<ShaderMethod> getMethods(string code)
        => retrieveToken(code, regex_method, match => getMethod(code, match));

    private static IReadOnlyList<T> retrieveToken<T>(string code, Regex regex, Func<Match, T> creatorFunc)
        where T : Token
    {
        var match = regex.Match(code);
        var matches = new List<T>();

        do
        {
            if (!match.Success)
                break;

            matches.Add(creatorFunc(match));
        }
        while ((match = match.NextMatch()).Success);

        return matches;
    }

    private static ShaderMethod getMethod(string code, Match match)
    {
        int current = match.Index;

        // Look for the first open bracket.
        while (code[current] != '{')
            current++;

        // Move to the open bracket's index.
        current++;

        int depth = 1;

        while (depth != 0)
        {
            if (code[current] == '{')
                depth++;

            if (code[current] == '}')
                depth--;

            current++;

            if (current > code.Length)
                throw new ArgumentException(@"Invalid shader method.", nameof(code));
        }

        return new ShaderMethod(code, match.Index, current - match.Index, shaderMethodMap[match.Groups["Name"].Value]);
    }

    private void appendShaderBuiltins(StringBuilder builder)
    {
        foreach (var builtin in Enum.GetValues<GlobalUniforms>())
            builder.AppendLine(FormatUniform("mat4", builtin.GetDescription()));
    }

    protected abstract class Token
    {
        public readonly int Start;
        public readonly int Length;
        public int End => Length - Start;
        public ReadOnlySpan<char> Value => source.AsSpan(Start, Length);

        private readonly string source;

        protected Token(string source, int start, int length)
        {
            Start = start;
            Length = length;
            this.source = source;
        }
    }

    private class ShaderOutput : Token
    {
        public ShaderOutput(string source, int start, int length)
            : base(source, start, length)
        {
        }
    }

    private class ShaderMethod : Token
    {
        public readonly ShaderStage Target;

        public ShaderMethod(string source, int start, int length, ShaderStage target)
            : base(source, start, length)
        {
            Target = target;
        }
    }

    protected class ShaderUniform : Token
    {
        public readonly string Type;
        public readonly string Name;

        public ShaderUniform(string source, int start, int length, string type, string name)
            : base(source, start, length)
        {
            Type = type;
            Name = name;
        }
    }

    private class ShaderAttribute : Token
    {
        public readonly string Type;
        public readonly string Name;

        public ShaderAttribute(string source, int start, int length, string type, string name)
            : base(source, start, length)
        {
            Type = type;
            Name = name;
        }
    }

    protected enum ShaderStage
    {
        Vertex,
        Fragment,
        Compute,
    }

    private static readonly Regex regex_attrib = regex_attrib_generator();
    private static readonly Regex regex_extern = regex_extern_generator();
    private static readonly Regex regex_method = regex_method_generator();
    private static readonly Regex regex_output = regex_output_generator();

    [GeneratedRegex(@"(?:attrib)\s+(?<Type>\w+)\s+(?<Name>\w+);", RegexOptions.Compiled)]
    private static partial Regex regex_attrib_generator();

    [GeneratedRegex(@"(?:uniform|extern)\s+(?<Type>\w+)\s+(?<Name>\w+);", RegexOptions.Compiled)]
    private static partial Regex regex_extern_generator();

    [GeneratedRegex(@"(?:void)\s*(?<Name>vert|frag|comp)\s*\(\)", RegexOptions.Compiled)]
    private static partial Regex regex_method_generator();

    [GeneratedRegex(@"SK_COLOR[0-7]", RegexOptions.Compiled)]
    private static partial Regex regex_output_generator();

    private static readonly Dictionary<string, ShaderStage> shaderMethodMap = new()
    {
        { "vert", ShaderStage.Vertex },
        { "comp", ShaderStage.Compute },
        { "frag", ShaderStage.Fragment },
    };

    private static readonly string defines = @"
#define extern uniform
#define SK_POSITION gl_Position
#define SK_POINT_SIZE gl_PointSize
#define SK_CLIP_DISTANCE gl_ClipDistance
#define SK_DRAW_ID gl_DrawID
#define SK_VERTEX_ID gl_VertexID
#define SK_VERTEX_BASE gl_BaseVertex
#define SK_INSTANCE_ID gl_InstanceID
#define SK_INSTANCE_BASE gl_BaseInstance
#define P_MATRIX g_internal_ProjMatrix
#define V_MATRIX g_internal_ViewMatrix
#define M_MATRIX g_internal_ModelMatrix
#define OBJECT_TO_VIEW g_internal_ViewMatrix * g_internal_ModelMatrix
#define OBJECT_TO_CLIP g_internal_ProjMatrix * g_internal_ViewMatrix * g_internal_ModelMatrix
";
}
