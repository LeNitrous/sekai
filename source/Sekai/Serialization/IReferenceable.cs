// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Serialization;

/// <summary>
/// Denotes that a given object can be referenced using a <see cref="Guid"/>.
/// </summary>
public interface IReferenceable
{
    /// <summary>
    /// The ID for this referenceable object.
    /// </summary>
    Guid Id { get; }
}
