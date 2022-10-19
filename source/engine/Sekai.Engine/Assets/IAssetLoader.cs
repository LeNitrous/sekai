// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Engine.Assets;

public interface IAssetLoader
{
}

public interface IAssetLoader<T> : IAssetLoader
{
    T Load(ReadOnlySpan<byte> data);
}
