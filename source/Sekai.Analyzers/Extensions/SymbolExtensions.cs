// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Microsoft.CodeAnalysis;

namespace Sekai.Analyzers.Extensions;

public static class SymbolExtensions
{
    public static string ToFullyQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public static string ToFullName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    public static string ToQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
    }
}
