// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Scenes;

/// <summary>
/// Used on properties of <see cref="Component"/> derived classes to establish dependencies between other components.
/// </summary>
/// <remarks>
/// Component dependency resolution is resolved when it has been attached to or other components has been attached to
/// or detached from a <see cref="Node"/>. <see cref="Component.IsReady"/> returns whether all dependencies have been
/// resolved. The components are resolved from its hosting <see cref="Node"/> and bindings may be created and/or be
/// destroyed at any time.
/// </remarks>
/// <example>
/// <code>
/// // Resolving a hard-dependency where if not found, throws an exception.
/// [Bind]
/// private MyComponent component { get; set; } = null!;
///
/// // Resolving a soft-dependency where if not found, returns null.
/// [Bind]
/// private MyComponent? component { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class BindAttribute : Attribute
{
}
