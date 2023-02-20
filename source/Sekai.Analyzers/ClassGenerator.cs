// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Sekai.Analyzers.Analysis;
using Sekai.Analyzers.Extensions;

namespace Sekai.Analyzers;

public abstract class ClassGenerator : IIncrementalGenerator
{
    protected abstract string BaseClassName { get; }
    protected abstract string AttributeName { get; }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targets = context.SyntaxProvider.CreateSyntaxProvider
        (
            static (node, _) => node is ClassDeclarationSyntax,
            static (context, _) => (ClassDeclarationSyntax)context.Node
        );

        var compile = context.CompilationProvider.Combine(targets.Collect());

        context.RegisterSourceOutput(compile, (context, source) => execute(source.Left, source.Right, context));
    }

    protected abstract void Generate(Candidate candidate, StringBuilder builder);

    private void execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
            return;

        var candidates = getCandidatesForGeneration(compilation, classes.Distinct(), context).ToImmutableArray();

        if (candidates.IsDefaultOrEmpty)
            return;

        var builder = new StringBuilder();
        builder.AppendLine(@"// <auto-generated/>");
        builder.AppendLine();
        builder.AppendLine(@"#nullable enable");
        builder.AppendLine(@"#pragma warning disable 0618");
        builder.AppendLine();

        foreach (var candidate in candidates)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            Generate(candidate, builder);
            builder.AppendLine();
        }

        context.AddSource($"{GetType().Name}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private IEnumerable<Candidate> getCandidatesForGeneration(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        foreach (var node in classes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var model = compilation.GetSemanticModel(node.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(node, context.CancellationToken);

            if (symbol is null)
                continue;

            if (IsCandidateForGeneration(compilation, node, model, symbol, context))
                yield return getCandidateForGeneration(symbol, context);
        }
    }

    private Candidate getCandidateForGeneration(INamedTypeSymbol symbol, SourceProductionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        return new(symbol, symbol.ContainingNamespace, getCandidateMembersForGeneration(symbol.GetMembers(), context).ToArray());
    }

    private IEnumerable<CandidateMember> getCandidateMembersForGeneration(IEnumerable<ISymbol> members, SourceProductionContext context)
    {
        foreach (var member in members)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (IsCandidateMemberForGeneration(member, context))
                yield return getCandidateMemberForGeneration(member, context);
        }
    }

    private CandidateMember getCandidateMemberForGeneration(ISymbol symbol, SourceProductionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        return new(symbol, symbol.ContainingNamespace);
    }

    protected virtual bool IsCandidateMemberForGeneration(ISymbol symbol, SourceProductionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        return symbol.GetAttributes().Any(a => a.AttributeClass is not null && a.AttributeClass.ToFullName() == AttributeName);
    }

    protected virtual bool IsCandidateForGeneration(Compilation compilation, ClassDeclarationSyntax node, SemanticModel model, INamedTypeSymbol symbol, SourceProductionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var baseType = symbol.BaseType;

        if (baseType is null)
            return false;

        bool containsBaseClass = false;

        do
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (baseType.ToFullName() == BaseClassName)
            {
                containsBaseClass = true;
                break;
            }
        }
        while ((baseType = baseType?.BaseType) is not null);

        if (!containsBaseClass)
            return false;

        bool containsRequiredAttribute = false;

        foreach (var member in symbol.GetMembers())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            containsRequiredAttribute = member.GetAttributes().Any(a => a.AttributeClass is not null && a.AttributeClass.ToFullName() == AttributeName);

            if (containsRequiredAttribute)
                break;
        }

        if (!containsRequiredAttribute)
            return false;

        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.SEKAI_REQUIRE_PARTIAL_CLASS, node.Identifier.GetLocation()));
            return false;
        }

        if (!model.GetNullableContext(node.SpanStart).HasFlag(NullableContext.Enabled))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticRules.SEKAI_ENABLE_NULLABILITY, node.Identifier.GetLocation()));
            return false;
        }

        return true;
    }

    protected readonly struct Candidate
    {
        public readonly INamedTypeSymbol Symbol;
        public readonly INamespaceSymbol Namespace;
        public readonly IReadOnlyList<CandidateMember> Members;

        public Candidate(INamedTypeSymbol symbol, INamespaceSymbol ns, IReadOnlyList<CandidateMember> members)
        {
            Symbol = symbol;
            Members = members;
            Namespace = ns;
        }
    }

    protected readonly struct CandidateMember
    {
        public readonly ISymbol Symbol;
        public readonly INamespaceSymbol Namespace;

        public CandidateMember(ISymbol symbol, INamespaceSymbol ns)
        {
            Symbol = symbol;
            Namespace = ns;
        }
    }
}
