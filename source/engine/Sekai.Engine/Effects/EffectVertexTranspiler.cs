// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;

namespace Sekai.Engine.Effects;

public class EffectVertexTranspiler : EffectTranspiler
{
    protected override string EntryPoint { get; } = "vert";
    protected override string ReturnType { get; } = "vec4";

    protected override void WriteEntryPoint(EffectTranspilerContext context, StringBuilder builder)
    {
        foreach (var stage in context.Stages)
            builder.AppendLine($"vs_{stage.Name} = {stage.Name};");

        builder.AppendLine($"gl_Position = vert();");
    }

    protected override void WriteLayoutOutputs(EffectTranspilerContext context, StringBuilder builder)
    {
        for (int i = 0; i < context.Stages.Count; i++)
        {
            var stage = context.Stages[i];
            builder.AppendLine($"layout(location = {i}) out {stage.Type} vs_{stage.Name};");
        }
    }
}
