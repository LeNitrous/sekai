// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Services;

public interface IServiceContainer : IReadOnlyServiceContainer
{
    IReadOnlyServiceContainer? Parent { get; set; }
    void Cache(Type type, Func<object> creationFunc);
    void Cache(Type type, object instance);
    void Cache<T>(Func<T> creationFunc);
    void Cache<T>(T instance);
}
