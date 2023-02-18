// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sekai.Analyzers.Extensions;

namespace Sekai.Analyzers;

[Generator]
public class ComponentGenerator : ClassGenerator
{
    protected override string BaseClassName { get; } = @"Sekai.Scenes.Component";
    protected override string AttributeName { get; } = @"Sekai.Scenes.BindAttribute";

    protected override void Generate(Candidate candidate, StringBuilder builder)
    {
        var properties = candidate.Members.Select(m => m.Symbol).Cast<IPropertySymbol>();

        builder.AppendLine($"namespace {candidate.Namespace}");
        builder.AppendLine(@"{");
        builder.AppendLine($"   partial class {candidate.Symbol.ToQualifiedName()}");
        builder.AppendLine(@"   {");
        builder.AppendLine(@"       [global::System.ObsoleteAttribute]");
        builder.AppendLine($"       protected override ComponentBinder CreateBinder() => new _binder_(this, base.CreateBinder());");
        builder.AppendLine();
        builder.AppendLine(@"       [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Sekai.Analyzers.ComponentGenerator"", ""0.0.0.0"")]");
        builder.AppendLine($"       sealed class _binder_ : ComponentBinder<{candidate.Symbol.ToQualifiedName()}>");
        builder.AppendLine(@"       {");
        builder.AppendLine($"           public _binder_({candidate.Symbol.ToQualifiedName()} owner, ComponentBinder binder)");
        builder.AppendLine(@"               : base(owner, binder)");
        builder.AppendLine(@"           {");
        builder.AppendLine(@"           }");
        builder.AppendLine();
        builder.AppendLine(@"           public override bool IsComponentValid()");
        builder.AppendLine(@"           {");
        builder.AppendLine($"               return base.IsComponentValid(){(properties.Where(prop => prop.Type.NullableAnnotation != NullableAnnotation.Annotated).Any() ? " && " + string.Join(" &&", properties.Where(prop => prop.Type.NullableAnnotation != NullableAnnotation.Annotated).Select(prop => $"Owner.{prop.Name} is not null")) : string.Empty)};");
        builder.AppendLine(@"           }");
        builder.AppendLine();
        builder.AppendLine(@"           protected override bool Update(global::Sekai.Scenes.Component other, bool isBinding)");
        builder.AppendLine(@"           {");
        builder.AppendLine(@"               base.Update(other, isBinding);");
        builder.AppendLine();
        builder.AppendLine(@"               switch (other)");
        builder.AppendLine(@"               {");

        foreach (var prop in properties)
        {
            builder.AppendLine($"                   case {prop.Type.ToFullyQualifiedName()} c:");
            builder.AppendLine($"                       Owner.{prop.Name} = isBinding ? c : null!;");
            builder.AppendLine(@"                       return true;");
            builder.AppendLine();
        }

        builder.AppendLine($"                   default:");
        builder.AppendLine(@"                       return false;");
        builder.AppendLine(@"               }");
        builder.AppendLine(@"           }");
        builder.AppendLine(@"       }");
        builder.AppendLine(@"   }");
        builder.AppendLine(@"}");
    }

    protected override bool IsCandidateMemberForGeneration(ISymbol symbol, SourceProductionContext context)
    {
        return symbol is IPropertySymbol && base.IsCandidateMemberForGeneration(symbol, context);
    }
}
