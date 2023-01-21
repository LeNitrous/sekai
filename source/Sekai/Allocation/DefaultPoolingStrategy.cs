// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Allocation;

/// <summary>
/// The default pooling strategy.
/// </summary>
/// <typeparam name="T">An object who has a default parameterless constructor.</typeparam>
public class DefaultPoolingStrategy<T> : ObjectPoolingStrategy<T>
    where T : notnull, new()
{
    public override T Create() => new();

    public override bool Return(T obj) => true;
}
