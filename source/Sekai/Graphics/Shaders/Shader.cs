// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text;
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
    private readonly Dictionary<string, IUniform> uniforms = new();

    /// <summary>
    /// Creates a new shader from code.
    /// </summary>
    public Shader(string code)
    {
        Code = code;

        string transformed = code;

        var match = regex_attr.Match(transformed);

        var pass = new StringBuilder();
        var attribsIns = new StringBuilder();
        var attribsOut = new StringBuilder();
        int attribsEnd = 0;
        int attribsStart = -1;

        do
        {
            if (!match.Success)
                break;

            string type = match.Groups["Type"].Value;
            string name = match.Groups["Name"].Value;

            pass.AppendLine($"v_internal_{name} = {name};");
            attribsIns.AppendLine($"in {type} {name};");
            attribsOut.AppendLine($"out {type} v_internal_{name};");

            if (attribsStart == -1)
                attribsStart = match.Index;

            attribsEnd = match.Index + match.Length;
        }
        while ((match = match.NextMatch()).Success);

        transformed = transformed[..attribsStart] + attribsIns + attribsOut + transformed[attribsEnd..];
        transformed = transformed.Trim();

        if (regex_vert.IsMatch(transformed) && regex_frag.IsMatch(transformed))
        {
            string vert = string.Concat(template_glsl, template_vert);
            vert = vert.Replace("{{ pass }}", pass.ToString());
            vert = vert.Replace("{{ content }}", transformed);

            string frag = string.Concat(template_glsl, template_frag);
            frag = frag.Replace("{{ content }}", transformed);

            Native = Context.Factory.CreateShader(vert, frag);
        }

        if (regex_comp.IsMatch(transformed))
        {
            string comp = string.Concat(template_glsl, template_comp);
            comp = comp.Replace("{{ content }}", transformed);

            Native = Context.Factory.CreateShader(comp);
        }

        if (Native is null)
            throw new ArgumentException(@"Invalid shader code.");

        foreach (var uniform in Native.Uniforms)
        {
            if (Context.Uniforms.HasUniform(uniform.Name))
            {
                var global = Context.Uniforms.GetUniform(uniform.Name);
                global.Attach(uniform);
                uniforms.Add(global.Name, global);
            }
            else
            {
                uniforms.Add(uniform.Name, uniform);
            }
        }
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
            if (Context.Uniforms.HasUniform(uniform.Name))
            {
                var global = Context.Uniforms.GetUniform(uniform.Name);
                global.Detach(uniform);
            }
        }

        uniforms.Clear();
        Native.Dispose();
    }

    private static readonly Regex regex_attr = regex_attr_generator();
    private static readonly Regex regex_vert = regex_vert_generator();
    private static readonly Regex regex_frag = regex_frag_generator();
    private static readonly Regex regex_comp = regex_comp_generator();

    private static readonly string template_glsl = @"
#version 330 core
#define extern uniform

{{ content }}

";

    private static readonly string template_vert = @"
void main()
{
{{ pass }}
gl_Position = vert();
}
";

    private static readonly string template_frag = @"
out vec4 v_internal_Color;

void main()
{
v_internal_Color = frag();
}
";

    private static readonly string template_comp = @"
void main()
{
comp();
}
";

    [GeneratedRegex("(?:attrib)\\s*(?<Type>\\w+)\\s*(?<Name>\\w+);", RegexOptions.Compiled)]
    private static partial Regex regex_attr_generator();

    [GeneratedRegex("(?:vec4)\\s*(?:vert)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_vert_generator();

    [GeneratedRegex("(?:vec4)\\s*(?:frag)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_frag_generator();

    [GeneratedRegex("(?:void)\\s*(?:comp)\\(\\)", RegexOptions.Compiled)]
    private static partial Regex regex_comp_generator();
}
