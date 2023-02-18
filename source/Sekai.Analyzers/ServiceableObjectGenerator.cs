// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sekai.Analyzers.Extensions;

namespace Sekai.Analyzers;

[Generator]
public class ServiceableObjectGenerator : ClassGenerator
{
    protected override string BaseClassName { get; } = @"Sekai.ServiceableObject";
    protected override string AttributeName { get; } = @"Sekai.Allocation.ResolvedAttribute";

    protected override void Generate(Candidate candidate, StringBuilder builder)
    {
        var properties = candidate.Members.Select(m => m.Symbol).Cast<IPropertySymbol>();

        builder.AppendLine($"namespace {candidate.Namespace}");
        builder.AppendLine(@"{");
        builder.AppendLine($"   partial class {candidate.Symbol.ToQualifiedName()}");
        builder.AppendLine(@"   {");
        builder.AppendLine(@"       [global::System.ObsoleteAttribute]");
        builder.AppendLine($"       protected override ServiceContractResolver CreateContractResolver() => new _resolver_(this, base.CreateContractResolver());");
        builder.AppendLine();
        builder.AppendLine(@"       [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Sekai.Analyzers.ServiceableObjectGenerator"", ""0.0.0.0"")]");
        builder.AppendLine($"       sealed class _resolver_ : ServiceContractResolver<{candidate.Symbol.ToQualifiedName()}>");
        builder.AppendLine(@"       {");

        foreach (var prop in properties)
        {
            if (prop.Type.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
            {
                builder.AppendLine($"           global::System.Collections.Generic.IEnumerable<global::Sekai.Allocation.ServiceContract> {prop.Name};");
            }
            else
            {
                if (prop.Type.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    builder.AppendLine($"           global::Sekai.Allocation.ServiceContract? {prop.ToFullName()};");
                }
                else
                {
                    builder.AppendLine($"           global::Sekai.Allocation.ServiceContract {prop.Name};");
                }
            }
        }

        builder.AppendLine();
        builder.AppendLine($"           public _resolver_({candidate.Symbol.ToQualifiedName()} target, ServiceContractResolver @base)");
        builder.AppendLine(@"               : base(target, @base)");
        builder.AppendLine(@"           {");

        foreach (var prop in properties)
        {
            if (prop.Type.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
            {
                builder.AppendLine($"               this.{prop.Name} = global::Sekai.Host.Current.Services.LocateAll<{prop.Type.ToFullyQualifiedName()}>();");
            }
            else
            {
                builder.AppendLine($"               this.{prop.Name} = global::Sekai.Host.Current.Services.Locate<{prop.Type.ToFullyQualifiedName()}>({(prop.Type.NullableAnnotation != NullableAnnotation.Annotated).ToString().ToLowerInvariant()});");
            }
        }

        builder.AppendLine(@"           }");
        builder.AppendLine();
        builder.AppendLine(@"           public override void Resolve()");
        builder.AppendLine(@"           {");
        builder.AppendLine(@"               base.Resolve();");

        foreach (var prop in properties)
        {
            if (prop.Type.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
            {
                builder.AppendLine($"               this.Target.{prop.Name} = global::System.Linq.Enumerable.Cast<{prop.Type.ToFullyQualifiedName()}>(global::System.Linq.Enumerable.Select(this.{prop.ToFullName()}, c => c.Instance));");
            }
            else
            {
                if (prop.Type.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    builder.AppendLine($"               this.Target.{prop.Name} = this.{prop.Name}?.Instance as {prop.Type.ToFullyQualifiedName()};");
                }
                else
                {
                    builder.AppendLine($"               this.Target.{prop.Name} = ({prop.Type.ToFullyQualifiedName()})this.{prop.Name}.Instance;");
                }
            }
        }

        builder.AppendLine(@"           }");
        builder.AppendLine();
        builder.AppendLine(@"           protected override void Dispose(bool disposing)");
        builder.AppendLine(@"           {");
        builder.AppendLine(@"               if (disposing)");
        builder.AppendLine(@"               {");

        foreach (var prop in properties)
        {
            if (prop.Type.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
            {
                builder.AppendLine($"                   foreach (var _contract_{prop.Name} in this.{prop.Name})");
                builder.AppendLine(@"                   {");
                builder.AppendLine($"                       _contract_{prop.Name}.Dispose();");
                builder.AppendLine(@"                   }");
                builder.AppendLine();
            }
            else
            {
                if (prop.Type.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    builder.AppendLine($"                   this.{prop.Name}?.Dispose();");
                }
                else
                {
                    builder.AppendLine($"                   this.{prop.Name}.Dispose();");
                }
            }
        }

        builder.AppendLine(@"               }");
        builder.AppendLine();
        builder.AppendLine(@"               base.Dispose(disposing);");
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
