// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Services;

public interface IReadOnlyServiceContainer
{
    T? Resolve<T>([DoesNotReturnIf(true)] bool required);
}
