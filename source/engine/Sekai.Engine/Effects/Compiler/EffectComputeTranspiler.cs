using System.Text;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Effects.Compiler;

public class EffectComputeTranspiler : EffectTranspiler
{
    protected override string EntryPoint => @"comp";
    protected override string ReturnType => @"void";

    protected override void WriteMainMethod(StringBuilder builder, EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        builder.AppendLine($"comp();");
    }
}
