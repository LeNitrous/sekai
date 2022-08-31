// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;

namespace Sekai.Engine.Effects;

public class EffectFragmentTranspiler : EffectTranspiler
{
    protected override string EntryPoint { get; } = "frag";
    protected override string ReturnType { get; } = "vec4";

    protected override void WriteEntryPoint(EffectTranspilerContext context, StringBuilder builder)
    {
        builder.AppendLine("f_Color = frag();");
    }

    protected override void WriteLayoutOutputs(EffectTranspilerContext context, StringBuilder builder)
    {
        builder.AppendLine("layout(location = 0) out vec4 f_Color;");
    }
}
