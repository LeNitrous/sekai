// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sekai.Framework;

namespace Sekai.Engine.Effects;

public abstract class EffectTranspiler : FrameworkObject
{
    /// <summary>
    /// The entry point method name.
    /// </summary>
    protected abstract string EntryPoint { get; }

    /// <summary>
    /// The entry point method return type.
    /// </summary>
    protected abstract string ReturnType { get; }

    public EffectTranspileResult Transpile(string source)
    {
        var methodsMatch = regex_method.Match(source);
        bool foundEntryPoint = false;

        if (methodsMatch.Success)
        {
            do
            {
                if (methodsMatch.Groups["Name"].Value.Trim() == EntryPoint &&
                    methodsMatch.Groups["Type"].Value.Trim() == ReturnType)
                {
                    foundEntryPoint = true;
                    break;
                }

            }
            while ((methodsMatch = methodsMatch.NextMatch()).Success);
        }

        if (!foundEntryPoint)
            throw new EffectTranspileException(@"Did not find suitable entry point.");

        var structures = new Dictionary<string, string[]>();
        var structuresMatch = regex_struct.Match(source);

        if (structuresMatch.Success)
        {
            do
            {
                string name = structuresMatch.Groups["Name"].Value.Trim();
                string[] types = structuresMatch.Groups["Type"].Captures.Select(c => c.Value.Trim()).ToArray();
                structures.Add(name, types);
            }
            while ((structuresMatch = structuresMatch.NextMatch()).Success);
        }

        var typeDictionary = new GLSLTypeDictionary(structures);

        var stages = new List<EffectMember>();
        var resources = new List<EffectMember>();
        var membersMatch = regex_member.Match(source);

        if (membersMatch.Success)
        {
            do
            {
                string name = membersMatch.Groups["Name"].Value.Trim();
                string type = membersMatch.Groups["Type"].Value.Trim();
                string readOnly = membersMatch.Groups["ReadOnly"].Value.Trim();
                string qualifier = membersMatch.Groups["Qualifier"].Value.Trim();

                var flags = EffectMemberFlags.None;

                switch (qualifier)
                {
                    case "stage":
                        flags |= EffectMemberFlags.Staged;
                        break;

                    case "buffer":
                        flags |= EffectMemberFlags.Buffer;
                        break;

                    case "uniform":
                        flags |= EffectMemberFlags.Uniform;
                        break;
                }

                if (flags.HasFlag(EffectMemberFlags.Buffer) && !string.IsNullOrEmpty(readOnly))
                    flags |= EffectMemberFlags.ReadWrite;

                if (type.Contains("Cube"))
                    flags |= EffectMemberFlags.Cubemap;

                if (type.Contains("image"))
                {
                    flags &= ~EffectMemberFlags.Uniform;
                    flags |= EffectMemberFlags.Texture | EffectMemberFlags.ReadWrite;
                }

                if (type.Contains("sampler"))
                {
                    flags &= ~EffectMemberFlags.Uniform;
                    flags |= EffectMemberFlags.Sampler;
                }

                if (type.Contains("texture"))
                {
                    flags &= ~EffectMemberFlags.Uniform;
                    flags |= EffectMemberFlags.Texture;
                }

                int stride = !Regex.IsMatch(type, @"[iu]?(?:sampler|image|texture)(?:[123]D|Cube)?(?:MS)?(?:Array|Rect)?(?:Shadow)?") ? typeDictionary.GetStride(type) : 0;
                var member = EffectMember.Create(name, type, stride, flags);

                if (flags.HasFlag(EffectMemberFlags.Staged))
                {
                    stages.Add(member);
                }
                else
                {
                    resources.Add(member);
                }

                source = source.Replace(membersMatch.Value, string.Empty);
            }
            while ((membersMatch = membersMatch.NextMatch()).Success);
        }

        var context = new EffectTranspilerContext(stages, resources);

        var builder = new StringBuilder();
        builder.AppendLine("#version 450");
        WriteLayoutInputs(context, builder);
        WriteLayoutOutputs(context, builder);
        builder.AppendLine(source);
        builder.AppendLine("void main() {");
        WriteEntryPoint(context, builder);
        builder.AppendLine("}");

        return new EffectTranspileResult(builder.ToString(), stages.ToArray(), resources.ToArray());
    }

    protected abstract void WriteEntryPoint(EffectTranspilerContext context, StringBuilder builder);

    protected virtual void WriteLayoutInputs(EffectTranspilerContext context, StringBuilder builder)
    {
        for (int i = 0; i < context.Resources.Count; i++)
        {
            var resource = context.Resources[i];

            if (resource.Flags.HasFlag(EffectMemberFlags.Buffer))
            {
                builder.AppendLine($"layout(set = 0, binding = {i}) {(resource.Flags.HasFlag(EffectMemberFlags.ReadWrite) ? string.Empty : "readonly ")}buffer b_internal_{resource.Name}");
                builder.AppendLine("{");
                builder.AppendLine($"{resource.Type} {resource.Name}[];");
                builder.AppendLine("};");
            }

            if (resource.Flags.HasFlag(EffectMemberFlags.Uniform))
            {
                builder.AppendLine($"layout(set = 0, binding = {i}) uniform u_internal_{resource.Name}");
                builder.AppendLine("{");
                builder.AppendLine($"{resource.Type} {resource.Name};");
                builder.AppendLine("};");
            }

            if (resource.Flags.HasFlag(EffectMemberFlags.Texture) || resource.Flags.HasFlag(EffectMemberFlags.Sampler))
            {
                builder.AppendLine($"layout(set = 0, binding = {i}) uniform {resource.Type} {resource.Name};");
            }
        }


        for (int i = 0; i < context.Stages.Count; i++)
        {
            var stage = context.Stages[i];
            builder.AppendLine($"layout(location = {i}) in {stage.Type} {stage.Name};");
        }
    }

    protected virtual void WriteLayoutOutputs(EffectTranspilerContext context, StringBuilder builder)
    {
    }

    private static readonly Regex regex_method = new(@"(?<Type>\w+)\s+(?<Name>\w+)\(\)", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex regex_struct = new(@"(?>struct)\s+(?<Name>\w+)\s*{\s*(?:(?<MemberType>\w+)\s+(?:\w+);\s+)+\s*}", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex regex_member = new(@"(?>(?<Qualifier>uniform|stage|(?<ReadOnly>readonly\s+)?buffer))\s+(?<Type>\w+)\s+(?<Name>\w+);", RegexOptions.Multiline | RegexOptions.Compiled);

    private class GLSLTypeDictionary
    {
        private readonly Dictionary<string, int> strides = new();
        private readonly Dictionary<string, string[]> structDefinitions;

        public GLSLTypeDictionary(Dictionary<string, string[]> structDefinitions)
        {
            strides.Add("int", sizeof(int));
            strides.Add("uint", sizeof(uint));
            strides.Add("bool", sizeof(bool));
            strides.Add("vec2", sizeof(float) * 2);
            strides.Add("vec3", sizeof(float) * 3);
            strides.Add("vec4", sizeof(float) * 4);
            strides.Add("mat2", sizeof(float) * 2 * 2);
            strides.Add("mat3", sizeof(float) * 3 * 3);
            strides.Add("mat4", sizeof(float) * 4 * 4);
            strides.Add("float", sizeof(float));
            strides.Add("mat2x2", sizeof(float) * 2 * 2);
            strides.Add("mat2x3", sizeof(float) * 2 * 3);
            strides.Add("mat2x4", sizeof(float) * 2 * 4);
            strides.Add("mat3x2", sizeof(float) * 3 * 2);
            strides.Add("mat3x3", sizeof(float) * 3 * 3);
            strides.Add("mat3x4", sizeof(float) * 3 * 4);
            strides.Add("mat4x2", sizeof(float) * 4 * 2);
            strides.Add("mat4x3", sizeof(float) * 4 * 3);
            strides.Add("mat4x4", sizeof(float) * 4 * 4);
            strides.Add("double", sizeof(double));

            this.structDefinitions = structDefinitions;

            foreach ((string name, string[] members) in structDefinitions)
            {
                if (strides.ContainsKey(name))
                    continue;

                strides.Add(name, getSizeForStruct(members));
            }
        }

        public int GetStride(string name)
        {
            if (!strides.TryGetValue(name, out int value))
                throw new InvalidOperationException();

            return value;
        }

        private int getSizeForStruct(string[] members)
        {
            int size = 0;

            foreach (string member in members)
            {
                if (!strides.TryGetValue(member, out int value))
                {
                    if (!structDefinitions.TryGetValue(member, out string[]? theirMembers))
                        throw new InvalidOperationException();

                    value = getSizeForStruct(theirMembers);
                    strides.Add(member, value);
                }

                size += value;
            }

            return size;
        }
    }
}
