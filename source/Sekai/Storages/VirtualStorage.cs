// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> which virtualizes paths to lead to other storages.
/// </summary>
internal sealed class VirtualStorage : Storage
{
    private bool isDisposed;
    private readonly IReadOnlyDictionary<string, Storage> storages;

    public VirtualStorage(IReadOnlyDictionary<string, Storage> storages)
    {
        this.storages = storages;
    }

    public override bool CreateDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return false;
        }

        return storage.CreateDirectory(subPath);
    }

    public override bool Delete(string path)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return false;
        }

        return storage.Delete(subPath);
    }

    public override bool DeleteDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return false;
        }

        return storage.DeleteDirectory(subPath);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return Enumerable.Empty<string>();
        }

        return storage.EnumerateDirectories(subPath, pattern, options);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return Enumerable.Empty<string>();
        }

        return storage.EnumerateFiles(subPath, pattern, options);
    }

    public override bool Exists(string path)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return false;
        }

        return storage.Exists(subPath);
    }

    public override bool ExistsDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            return false;
        }

        return storage.ExistsDirectory(subPath);
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!resolve(path, out string? subPath, out var storage))
        {
            throw new DirectoryNotFoundException();
        }

        return storage.Open(subPath, mode, access);
    }

    private bool resolve(string path, [NotNullWhen(true)] out string? subPath, [NotNullWhen(true)] out Storage? storage)
    {
        for (int i = path.Length - 1; i > 0; i--)
        {
            if (path[i] != Path.AltDirectorySeparatorChar)
                continue;

            subPath = path[..(i + 1)];

            if (!storages.TryGetValue(subPath, out storage))
                continue;

            subPath = path[i..];

            return true;
        }

        subPath = null;
        storage = default;

        return false;
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        foreach (var storage in storages.Values)
        {
            storage.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
