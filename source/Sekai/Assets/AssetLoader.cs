// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using Sekai.Allocation;
using Sekai.Storages;

namespace Sekai.Assets;

/// <summary>
/// Loads <see cref="IAsset"/>s.
/// </summary>
public sealed class AssetLoader : DependencyObject, IAssetLoaderRegistry
{
    [Resolved]
    private StorageContext storage { get; set; } = null!;

    private readonly Dictionary<string, IAssetLoader> loaders = new();

    /// <summary>
    /// Loads an asset from the given path using the current <see cref="StorageContext"/>.
    /// </summary>
    public T Load<T>(string path)
        where T : notnull, IAsset
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

        string ext = Path.GetExtension(path);

        if (string.IsNullOrEmpty(ext))
            throw new ArgumentException("Unable to determine file extension of the provided path.", nameof(path));

        if (!loaders.TryGetValue(ext, out var loader))
            throw new NotSupportedException($@"Loading of asset type ""{typeof(T)}"" is not supported.");

        if (loader is not IAssetLoader<T> typedLoader)
            throw new InvalidCastException($@"The asset loader is unable to load ""{typeof(T)}"" objects.");

        using var stream = storage.Open(path, FileMode.Open, FileAccess.Read);

        Span<byte> data = stackalloc byte[(int)stream.Length];

        if (stream.Read(data) <= 0)
            throw new InvalidOperationException(@"Failed to read stream.");

        return typedLoader.Load(data);
    }

    void IAssetLoaderRegistry.Register<T>(IAssetLoader<T> loader)
    {
        foreach (string ext in loader.Extensions)
        {
            string extension = ext[0] != '.' ? '.' + ext : ext;

            if (loaders.ContainsKey(extension))
                throw new InvalidOperationException(@$"There is an asset loader that already handles the ""{extension}"" extension.");

            loaders.Add(extension, loader);
        }
    }
}
