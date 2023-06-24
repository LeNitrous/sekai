// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Sekai.Framework.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> which merges other storages together.
/// </summary>
/// <remarks>
/// This type of storage is read-only and only files can be accessed.
/// </remarks>
public sealed class MergedStorage : Storage
{
    private bool isDisposed;
    private readonly HashSet<Storage> storages = new();

    /// <summary>
    /// Adds a storage.
    /// </summary>
    public void Add(Storage storage)
    {
        storages.Add(storage);
    }

    /// <summary>
    /// Removes a storage.
    /// </summary>
    public void Remove(Storage storage)
    {
        storages.Remove(storage);
    }

    public override bool CreateDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool Delete([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool DeleteDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override IEnumerable<string> EnumerateDirectories([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return Enumerable.Empty<string>();
    }

    public override IEnumerable<string> EnumerateFiles([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return storages.SelectMany(s => s.EnumerateFiles(path, pattern, options));
    }

    public override bool Exists([StringSyntax("Uri")] string path)
    {
        return storages.Any(s => s.Exists(path));
    }

    public override bool ExistsDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override Stream Open([StringSyntax("Uri")] string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        var storage = storages.FirstOrDefault(s => s.Exists(path)) ?? throw new FileNotFoundException(null, Path.GetFileName(path));
        return storage.Open(path, mode, access);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        storages.Clear();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
