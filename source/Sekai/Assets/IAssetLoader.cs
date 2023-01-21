// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Assets;

/// <summary>
/// An asset loader.
/// </summary>
public interface IAssetLoader
{
    /// <summary>
    /// The extensions this asset loader can load.
    /// </summary>
    string[] Extensions { get; }
}

/// <summary>
/// A typed asset loader.
/// </summary>
/// <typeparam name="T">The type of asset this can load.</typeparam>
public interface IAssetLoader<T> : IAssetLoader
    where T : notnull, IAsset
{
    /// <summary>
    /// Loads an asset from a byte span.
    /// </summary>
    T Load(ReadOnlySpan<byte> bytes);
}
