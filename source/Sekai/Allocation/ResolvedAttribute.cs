// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Allocation;

/// <summary>
/// A attribute used in properties of <see cref="DependencyObject"/> derived classes to resolve a dependency from a <see cref="ServiceLocator"/>.
/// </summary>
/// <remarks>
/// Dependency resolution occurs during object construction. The service resolved depends on the property's type. The dependency type depends on
/// whether a nullable operator is present or not. To define a hard dependency, define it without the nullable operator.
/// </remarks>
/// <example>
/// <code>
/// // Resolving a hard-dependency where if not found, throws an exception.
/// [Resolved]
/// private MyService service { get; set; } = null!;
///
/// // Resolving a soft-dependency where if not found, returns null.
/// [Resolved]
/// private MyService? service { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ResolvedAttribute : Attribute
{
}
