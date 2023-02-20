// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Microsoft.CodeAnalysis;

namespace Sekai.Analyzers.Analysis;

public static class DiagnosticRules
{
    public static DiagnosticDescriptor SEKAI_ENABLE_NULLABILITY = new
    (
        "SEKAI0001",
        "This class is candidate for dependency injection and should have enabled nullables",
        "This class is candidate for dependency injection and should have enabled nullables",
        "Design",
        DiagnosticSeverity.Warning,
        true,
        "Enable nullability on this class to allow optional dependencies to be injected."
    );

    public static DiagnosticDescriptor SEKAI_REQUIRE_PARTIAL_CLASS = new
    (
        "SEKAI0002",
        "This class is candidate for dependency injection and should be made partial",
        "This class is candidate for dependency injection and should be made partial",
        "Design",
        DiagnosticSeverity.Warning,
        true,
        "Classes that are candidate for dependency injection should be made partial to benefit from compile-time optimizations."
    );
}
