// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sekai.Storages;

namespace Sekai.Assets;

/// <summary>
/// Loads <see cref="IAsset"/>s.
/// </summary>
public sealed class AssetLoader : DisposableObject
{
    private readonly Storage storage;
    private readonly IReadOnlyList<IAssetLoader> loaders;
    private readonly IReadOnlyList<IReadOnlyList<string>> extensions;

    internal AssetLoader(Storage storage, IReadOnlyList<IAssetLoader> loaders, IReadOnlyList<IReadOnlyList<string>> extensions)
    {
        this.storage = storage;
        this.loaders = loaders;
        this.extensions = extensions;
    }

    /// <inheritdoc cref="Load{T}(Uri)"/>
    /// <param name="path">The path to the file.</param>
    public T Load<T>(string path)
        where T : notnull, IAsset
    {
        return Load<T>(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// Loads an asset from the given path.
    /// </summary>
    /// <typeparam name="T">The type of asset to load as.</typeparam>
    /// <param name="uri">The URI to the file.</param>
    /// <returns>The loaded asset.</returns>
    /// <exception cref="ArgumentException">Thrown when no asset loader can load <typeparamref name="T"/> objects.</exception>
    /// <exception cref="InvalidCastException">Thrown when an invalid <typeparamref name="T"/> is used against the URI's extension.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the stream failed to be read.</exception>
    public T Load<T>(Uri uri)
        where T : notnull, IAsset
    {
        string? extension = Path.GetExtension(uri.OriginalString);

        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("URI must have an extension.", nameof(uri));

        IAssetLoader? loader = null;

        for (int i = 0; i < loaders.Count; i++)
        {
            loader = loaders[i];

            if (extensions[i].Contains(extension))
                break;
        }

        if (loader is null)
            throw new ArgumentException($@"There is no asset loader that can load ""{typeof(T)}"" objects.", nameof(T));

        if (loader is not IAssetLoader<T>)
            throw new InvalidCastException($@"The ""{typeof(T)}"" asset loader is unable to load ""{extension}"".");

        using var stream = storage.Open(uri, FileMode.Open, FileAccess.Read);

        Span<byte> data = new byte[stream.Length];

        if (stream.Read(data) <= 0)
            throw new InvalidOperationException(@"Failed to read stream.");

        return ((IAssetLoader<T>)loader).Load(data);
    }
}
