// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;

namespace Sekai.Engine.Effects;

public class EffectComputeTranspiler : EffectTranspiler
{
    protected override string EntryPoint => "comp";
    protected override string ReturnType => "void";

    protected override void WriteEntryPoint(EffectTranspilerContext context, StringBuilder builder)
    {
        builder.AppendLine($"comp();");
    }
}
