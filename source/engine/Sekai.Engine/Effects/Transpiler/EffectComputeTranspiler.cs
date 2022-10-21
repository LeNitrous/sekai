// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;
using Sekai.Engine.Effects.Analysis;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Transpiler;

public class EffectComputeTranspiler : EffectTranspiler
{
    protected override string EntryPoint => @"comp";
    protected override string ReturnType => @"void";

    protected override void WriteMainMethod(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        builder.AppendLine($"comp();");
    }
}
