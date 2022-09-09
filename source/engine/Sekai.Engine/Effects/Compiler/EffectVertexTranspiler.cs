// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Compiler;

public class EffectVertexTranspiler : EffectTranspiler
{
    protected override string EntryPoint => @"vert";
    protected override string ReturnType => @"vec4";

    protected override void WriteLayoutOutputs(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        for (int i = 0; i < analysis.Stages.Length; i++)
        {
            var stage = analysis.Stages[i];
            builder.AppendLine($"layout(location = {i}) out {stage.Type} s_internal_{stage.Name};");
        }
    }

    protected override void WriteMainMethod(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        foreach (var stage in analysis.Stages)
        {
            builder.AppendLine($"s_internal_{stage.Name} = {stage.Name};");
        }

        builder.AppendLine(@"gl_Position = vert();");
    }
}
