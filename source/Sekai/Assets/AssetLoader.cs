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
    private VirtualStorage storage { get; set; } = null!;

    private readonly Dictionary<string, IAssetLoader> loaders = new();

    /// <inheritdoc cref="Load{T}(Uri)"/>
    /// <param name="path">The path to the file.</param>
    public T Load<T>(string path)
        where T : notnull, IAsset
    {
        return Load<T>(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// Loads an asset from the given path using the current <see cref="VirtualStorage"/>.
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
        IAssetLoader? loader = null;

        foreach ((string extension, var assetLoader) in loaders)
        {
            if (uri.OriginalString.EndsWith(extension))
            {
                loader = assetLoader;
                break;
            }
        }

        if (loader is null)
            throw new ArgumentException($@"There is no asset loader that can load ""{typeof(T)}"" objects.", nameof(T));

        if (loader is not IAssetLoader<T> typedLoader)
            throw new InvalidCastException($@"The asset loader is unable to load ""{typeof(T)}"" objects.");

        using var stream = storage.Open(uri, FileMode.Open, FileAccess.Read);

        int streamLength = (int)stream.Length;
        Span<byte> data = streamLength > RuntimeInfo.MaximumStackCapacity ? new byte[streamLength] : stackalloc byte[streamLength];

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
