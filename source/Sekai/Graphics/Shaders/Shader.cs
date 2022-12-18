// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A program that is executed at a specified stage on the GPU.
/// </summary>
public sealed partial class Shader : GraphicsObject
{
    /// <summary>
    /// The shader type.
    /// </summary>
    public ShaderType Type => Native.Type;

    /// <summary>
    /// The shader code.
    /// </summary>
    public readonly string Code;

    internal readonly INativeShader Native;
    internal IReadOnlyDictionary<string, IUniform> Uniforms => uniforms;

    private readonly Dictionary<string, IUniform> uniforms;

    /// <summary>
    /// Creates a new shader from code.
    /// </summary>
    public Shader(string code)
    {
        Code = code;
        Native = createNativeShader(code);
        uniforms = createUniformMap(Native);
    }

    /// <summary>
    /// Makes this shader the current.
    /// </summary>
    public void Bind()
    {
        Context.BindShader(this);
    }

    /// <summary>
    /// Makes this shader not the current.
    /// </summary>
    public void Unbind()
    {
        Context.UnbindShader(this);
    }

    /// <summary>
    /// Retrieves a declared uniform.
    /// </summary>
    public IUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        if (!uniforms.TryGetValue(name, out var uniform))
            throw new Exception($@"There is no shader uniform named ""{name}"".");

        if (uniform is GlobalUniform<T>)
            throw new Exception(@"Retrieved uniform is a global uniform.");

        if (uniform is not IUniform<T> u)
            throw new InvalidCastException(@"Unable to cast uniform to the given type.");

        return u;
    }

    protected sealed override void Destroy()
    {
        foreach (var uniform in Native.Uniforms)
        {
            if (Context.Uniforms.TryGetUniform(uniform.Name, out var global))
                global.Detach(uniform);
        }

        uniforms.Clear();
        Native.Dispose();
    }

    private Dictionary<string, IUniform> createUniformMap(INativeShader native)
    {
        var uniforms = new Dictionary<string, IUniform>();

        foreach (var uniform in native.Uniforms)
        {
            if (Context.Uniforms.TryGetUniform(uniform.Name, out var global))
            {
                global.Attach(uniform);
                uniforms.Add(global.Name, global);
            }
            else
            {
                uniforms.Add(uniform.Name, uniform);
            }
        }

        return uniforms;
    }

    private INativeShader createNativeShader(string code)
    {
        var attribs = getAttribCalls(code);
        var vertMatch = regex_vert.Match(code);
        var fragMatch = regex_frag.Match(code);
        var compMatch = regex_comp.Match(code);

        if (vertMatch.Success && fragMatch.Success)
        {
            string vertMethod = getMethod(code, vertMatch.Index);
            string fragMethod = getMethod(code, fragMatch.Index);

            string vertBody = string.Empty;
            string fragHead = string.Empty;
            string vertContent = code;
            string fragContent = code;

            int attribLoc = 0;
            foreach (var attrib in attribs)
            {
                string line = attrib.Line.ToString();
                string a = $"layout(location = {attribLoc}) in {attrib.Type} {attrib.Name};";
                string b = $"layout(location = {attribLoc}) out {attrib.Type} v_internal_{attrib.Name};";

                vertBody += $"v_internal_{attrib.Name} = {attrib.Name};" + Environment.NewLine;
                vertContent = vertContent.Replace(line, string.Join(Environment.NewLine, a, b));
                fragContent = fragContent.Replace(line, a);

                attribLoc++;
            }

            int colorMatches = regex_color.Matches(fragMethod).Count;
            for (int colorLoc = 0; colorLoc < colorMatches; colorLoc++)
                fragHead += $"layout(location = {colorLoc}) out vec4 SK_COLOR{colorLoc};" + Environment.NewLine;

            string vert = string.Concat(template_glsl, template_vert);
            vert = vert.Replace("{{ body }}", vertBody);
            vert = vert.Replace("{{ content }}", vertContent);
            vert = vert.Replace(fragMethod, string.Empty);

            string frag = string.Concat(template_glsl, template_frag);
            frag = frag.Replace("{{ head }}", fragHead);
            frag = frag.Replace("{{ content }}", fragContent);
            frag = frag.Replace(vertMethod, string.Empty);

            return Context.Factory.CreateShader(vert, frag);
        }

        if (compMatch.Success)
        {
            string compContent = code;

            int attribLoc = 0;
            foreach (var attrib in attribs)
            {
                compContent = compContent.Replace(attrib.Line.ToString(), $"layout(location = {attribLoc}) in {attrib.Type} {attrib.Name};");
                attribLoc++;
            }

            string comp = string.Concat(template_glsl, template_comp);
            comp = comp.Replace("{{ content }}", compContent);

            return Context.Factory.CreateShader(comp);
        }

        throw new ArgumentException(@"Unable to determine shader type from code", nameof(code));
    }

    private static string getMethod(string code, int start)
    {
        int current = start;

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
                throw new ArgumentException(@"Invalid shader code.", nameof(code));
        }

        // This should be a span as well but unfortunately they removed the Replace APIs for ReadOnlySpan<char>
        // Replace this as a span once this has been added: https://github.com/dotnet/runtime/issues/29758
        return code[start..current];
    }

    private static IEnumerable<ShaderAttribute> getAttribCalls(string code)
    {
        var match = regex_attr.Match(code);
        var memory = code.AsMemory();

        do
        {
            if (!match.Success)
                yield break;

            var type = match.Groups["Type"];
            var name = match.Groups["Name"];

            yield return new ShaderAttribute
            (
                memory.Slice(match.Index, match.Length),
                memory.Slice(name.Index, name.Length),
                memory.Slice(type.Index, type.Length)
            );
        }
        while ((match = match.NextMatch()).Success);
    }

    private static readonly Regex regex_attr = regex_attr_generator();
    private static readonly Regex regex_vert = regex_vert_generator();
    private static readonly Regex regex_frag = regex_frag_generator();
    private static readonly Regex regex_comp = regex_comp_generator();
    private static readonly Regex regex_color = regex_color_generator();

    [GeneratedRegex("(?:attrib)\\s*(?<Type>\\w+)\\s*(?<Name>\\w+);", RegexOptions.Compiled)]
    private static partial Regex regex_attr_generator();

    [GeneratedRegex("(?:void)\\s*(?:vert)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_vert_generator();

    [GeneratedRegex("(?:void)\\s*(?:frag)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_frag_generator();

    [GeneratedRegex("(?:void)\\s*(?:comp)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_comp_generator();

    [GeneratedRegex("SK_COLOR[0-7]", RegexOptions.Compiled)]
    private static partial Regex regex_color_generator();

    private static readonly string template_glsl = @"
#version 330 core
#extension GL_ARB_separate_shader_objects : enable
#define extern uniform
#define SK_POSITION gl_Position
";

    private static readonly string template_vert = @"
{{ content }}
void main()
{
{{ body }}
vert();
}
";

    private static readonly string template_frag = @"
{{ head }}
{{ content }}
void main()
{
frag();
}";
    private static readonly string template_comp = @"
{{ content }}
void main()
{
comp();
}";

    private readonly record struct ShaderAttribute(ReadOnlyMemory<char> Line, ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Type);
}
