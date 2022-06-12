// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Services;

public interface IReadOnlyServiceContainer
{
    IReadOnlyDictionary<Type, Func<object>> Cached { get; }
    T Resolve<T>([DoesNotReturnIf(true)] bool required = false);
    object Resolve(Type type, [DoesNotReturnIf(true)] bool required = false);
}
