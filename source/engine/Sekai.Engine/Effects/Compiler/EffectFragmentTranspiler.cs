// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Compiler;

public class EffectFragmentTranspiler : EffectTranspiler
{
    protected override string EntryPoint => @"frag";
    protected override string ReturnType => @"vec4";

    protected override void WriteLayoutOutputs(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        builder.AppendLine(@"layout(location = 0) out vec4 fs_Color;");
    }

    protected override void WriteMainMethod(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        builder.AppendLine(@"fs_Color = frag();");
    }
}
