// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Containers;

public interface IReadOnlyContainer
{
    /// <summary>
    /// Resolves instance of type.
    /// </summary>
    T Resolve<T>([DoesNotReturnIf(true)] bool required = true);

    /// <summary>
    /// Resolves instance of type.
    /// </summary>
    object Resolve(Type type, [DoesNotReturnIf(true)] bool required = true);
}
