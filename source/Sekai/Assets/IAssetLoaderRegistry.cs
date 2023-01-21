// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Assets;

/// <summary>
/// An interface that exposes methods for asset loader registry.
/// </summary>
public interface IAssetLoaderRegistry
{
    /// <summary>
    /// Registers an asset loader.
    /// </summary>
    /// <param name="loader">The asset loader to register.</param>
    void Register<T>(IAssetLoader<T> loader) where T : notnull, IAsset;
}
