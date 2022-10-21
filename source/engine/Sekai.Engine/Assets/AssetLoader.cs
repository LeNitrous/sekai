// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using Sekai.Framework;
using Sekai.Framework.Storage;

namespace Sekai.Engine.Assets;

public class AssetLoader : FrameworkObject
{
    private readonly VirtualStorage storage = Game.Resolve<VirtualStorage>();
    private readonly Dictionary<Type, IAssetLoader> loaders = new();

    /// <summary>
    /// Registers a loader for a given asset type.
    /// </summary>
    public void Register<T, U>()
        where U : IAssetLoader<T>
    {
        var instance = Activator.CreateInstance<U>();

        if (loaders.ContainsValue(instance))
            throw new InvalidOperationException();

        loaders.Add(typeof(T), instance);
    }

    /// <summary>
    /// Loads an asset from the virtual storage.
    /// </summary>
    public T Load<T>(string path)
    {
        using var stream = storage.Open(path, FileMode.Open, FileAccess.Read);
        return Load<T>(stream);
    }

    /// <summary>
    /// Loads an asset from a stream.
    /// </summary>
    public T Load<T>(Stream stream)
    {
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return Load<T>(memory.ToArray());
    }

    /// <summary>
    /// Loads an asset from a byte array.
    /// </summary>
    public T Load<T>(byte[] bytes)
    {
        return Load<T>(new Span<byte>(bytes));
    }

    /// <summary>
    /// Loads an asset from a span of bytes.
    /// </summary>
    public T Load<T>(ReadOnlySpan<byte> data)
    {
        var key = typeof(T);

        if (!loaders.ContainsKey(key))
            throw new InvalidOperationException();

        if (loaders[key] is not IAssetLoader<T> loader)
            throw new InvalidOperationException();

        return loader.Load(data);
    }
}
