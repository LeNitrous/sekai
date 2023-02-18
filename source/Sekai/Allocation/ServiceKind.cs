// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Allocation;

/// <summary>
/// Describes how the service was instantiated.
/// </summary>
public enum ServiceKind
{
    /// <summary>
    /// The service is already instantiated.
    /// </summary>
    /// <remarks>
    /// Object disposal must be handled manually by the instantiator.
    /// </remarks>
    Constant,

    /// <summary>
    /// The service is lazily instantiated.
    /// </summary>
    /// <remarks>
    /// Object disposal is handled by the <see cref="ServiceDescriptor"/>.
    /// </remarks>
    Lazy,

    /// <summary>
    /// The service is always instantiated.
    /// </summary>
    /// <remarks>
    /// Object disposal is handled by the <see cref="ServiceContract"/>.
    /// </remarks>
    Func,
}
