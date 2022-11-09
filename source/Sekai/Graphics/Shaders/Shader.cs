// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A program that is executed at a specified stage on the GPU.
/// </summary>
public sealed class Shader : FrameworkObject
{
    /// <summary>
    /// The shader type.
    /// </summary>
    public ShaderType Type => Native.Type;

    /// <summary>
    /// Gets whether this shader is currently bound.
    /// </summary>
    public bool IsBound { get; private set; }

    internal INativeShader Native { get; private set; }

    private string code;
    private readonly ShaderGlobals globals = Game.Resolve<ShaderGlobals>();
    private readonly GraphicsContext graphics = Game.Resolve<GraphicsContext>();
    private readonly IGraphicsFactory factory = Game.Resolve<IGraphicsFactory>();
    private readonly Dictionary<string, IUniform> uniforms = new();

    /// <summary>
    /// Creates a new shader from code.
    /// </summary>
    public Shader(string code)
    {
        this.code = code;
        prepareSource();
        prepareUniforms();
    }

    /// <summary>
    /// Makes this shader active.
    /// </summary>
    public void Bind()
    {
        if (IsBound)
            return;

        graphics.BindShader(this);
        IsBound = true;
    }

    /// <summary>
    /// Makes this shader
    /// </summary>
    public void Unbind()
    {
        if (!IsBound)
            return;

        graphics.UnbindShader(this);
        IsBound = false;
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

    private void prepareUniforms()
    {
        foreach (var uniform in Native.Uniforms)
        {
            if (globals.HasUniform(uniform.Name))
            {
                var global = globals.GetUniform(uniform.Name);
                global.Attach(uniform);
                uniforms.Add(global.Name, global);
            }
            else
            {
                uniforms.Add(uniform.Name, uniform);
            }
        }
    }

    [MemberNotNull("Native")]
    private void prepareSource()
    {
        var match = regex_attr.Match(code);

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

        code = code[..attribsStart] + attribsIns + attribsOut + code[attribsEnd..];
        code = code.Trim();

        if (regex_vert.IsMatch(code) && regex_frag.IsMatch(code))
        {
            string vert = string.Concat(template_glsl, template_vert);
            vert = vert.Replace("{{ pass }}", pass.ToString());
            vert = vert.Replace("{{ content }}", code);

            string frag = string.Concat(template_glsl, template_frag);
            frag = frag.Replace("{{ content }}", code);

            Native = factory.CreateShader(vert, frag);
        }

        if (regex_comp.IsMatch(code))
        {
            string comp = string.Concat(template_glsl, template_comp);
            comp = comp.Replace("{{ content }}", code);

            Native = factory.CreateShader(comp);
        }

        if (Native is null)
            throw new ArgumentException(@"Invalid shader code.");
    }

    protected sealed override void Destroy()
    {
        foreach (var uniform in Native.Uniforms)
        {
            if (globals.HasUniform(uniform.Name))
            {
                var global = globals.GetUniform(uniform.Name);
                global.Detach(uniform);
            }
        }

        uniforms.Clear();
        Native.Dispose();
    }

    private static readonly Regex regex_attr = new(@"(?:attrib)\s*(?<Type>\w+)\s*(?<Name>\w+);",RegexOptions.Compiled);
    private static readonly Regex regex_vert = new(@"(?:vec4)\s*(?:vert)\(\)", RegexOptions.Compiled);
    private static readonly Regex regex_frag = new(@"(?:vec4)\s*(?:frag)\(\)", RegexOptions.Compiled);
    private static readonly Regex regex_comp = new(@"(?:void)\s*(?:comp)\(\)", RegexOptions.Compiled);

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
}
