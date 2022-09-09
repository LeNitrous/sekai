// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Text;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Compiler;

public abstract class EffectTranspiler
{
    protected abstract string EntryPoint { get; }
    protected abstract string ReturnType { get; }

    public string Transpile(EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        if (!pass.Methods.Any(m => m.Name == EntryPoint))
            throw new Exception($"There is no main method found for {pass.Name} pass.");

        var mainMethod = pass.Methods.First(m => m.Name == EntryPoint);

        if (mainMethod.Type != ReturnType)
            throw new Exception($"The main method has an incorrect return type.");

        if (mainMethod.Parameters.Length != 0)
            throw new Exception($"The main method should not have any parameters.");

        var builder = new StringBuilder();

        builder.AppendLine("#version 450");

        foreach (var @struct in pass.Structs)
        {
            builder.AppendLine($"struct {@struct.Name}");
            builder.AppendLine("{");

            foreach (var member in @struct.Members)
            {
                builder.AppendLine($"{member.Type} {member.Name}{(string.IsNullOrEmpty(member.Size) ? string.Empty : $"[{member.Size}]")};");
            }

            builder.AppendLine("}");
        }

        for (int i = 0; i < analysis.Parameters.Length; i++)
        {
            var parameter = analysis.Parameters[i];

            if (parameter.Flags.HasFlag(EffectParameterFlags.Buffer))
            {
                builder.AppendLine($"layout(set = 0, binding = {i}) buffer b_internal_{parameter.Name}");
                builder.AppendLine(@"{");
                builder.AppendLine($"{parameter.Type} {parameter.Name};");
                builder.AppendLine(@"};");
            }

            if (parameter.Flags.HasFlag(EffectParameterFlags.Uniform))
            {
                if (parameter.Flags.HasFlag(EffectParameterFlags.Image) || parameter.Flags.HasFlag(EffectParameterFlags.Texture) || parameter.Flags.HasFlag(EffectParameterFlags.Sampler))
                {
                    builder.AppendLine($"layout(set = 0, binding = {i}) uniform {parameter.Type} {parameter.Name};");
                    continue;
                }

                builder.AppendLine($"layout(set = 0, binding = {i}) uniform u_internal_{parameter.Name}");
                builder.AppendLine(@"{");
                builder.AppendLine($"{parameter.Type} {parameter.Name};");
                builder.AppendLine(@"};");
            }
        }

        for (int i = 0; i < analysis.Stages.Length; i++)
        {
            var stage = analysis.Stages[i];
            builder.AppendLine($"layout(location = {i}) in {stage.Type} {stage.Name};");
        }

        WriteLayoutOutputs(builder, pass, analysis);

        foreach (var method in pass.Methods)
        {
            builder.AppendLine(method.Body);
        }

        builder.AppendLine("void main()");
        builder.AppendLine("{");
        WriteMainMethod(builder, pass, analysis);
        builder.AppendLine("}");

        return builder.ToString();
    }

    protected virtual void WriteLayoutOutputs(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
    }

    protected virtual void WriteMainMethod(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
    }
}
