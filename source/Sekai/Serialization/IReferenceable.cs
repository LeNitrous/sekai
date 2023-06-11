// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Serialization;

/// <summary>
/// An interface denoting an object can be referenced by an <see cref="Id"/>.
/// </summary>
public interface IReferenceable
{
    /// <summary>
    /// Gets the referenceable's identifier.
    /// </summary>
    Guid Id { get; }
}
