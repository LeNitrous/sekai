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
public sealed class MountableStorage : Storage
{
    private bool isDisposed;
    private readonly Dictionary<string, MountedStorage> storages = new();

    /// <summary>
    /// Mounts a storage at a given path.
    /// </summary>
    /// <param name="path">The path to be mounted</param>
    /// <param name="storage">The storage to mount.</param>
    /// <param name="writable">Whether the storage can be written to.</param>
    /// <exception cref="Exception">Thrown when a storage is already mounted at a given path.</exception>
    public void Mount([StringSyntax(StringSyntaxAttribute.Uri)] string path, Storage storage, bool writable = true)
    {
        path = makeRelativePath(path);

        if (storages.ContainsKey(path))
        {
            throw new ArgumentException($"There is a storage already mounted at {path}", nameof(path));
        }

        storages.Add(path, new MountedStorage(storage, writable));
    }

    /// <inheritdoc cref="Unmount(string, out Storage?)"/>
    public bool Unmount([StringSyntax(StringSyntaxAttribute.Uri)] string path)
    {
        return Unmount(path, out _);
    }

    /// <summary>
    /// Unmounts a given path.
    /// </summary>
    /// <param name="path">The path to unmount.</param>
    /// <param name="storage">The storage that was unmounted.</param>
    /// <returns><see langword="true"/> if the path was unmounted. <see langword="false"/> otherwise.</returns>
    public bool Unmount([StringSyntax(StringSyntaxAttribute.Uri)] string path, [NotNullWhen(true)] out Storage? storage)
    {
        path = makeRelativePath(path);

        if (!storages.Remove(path, out var mount))
        {
            storage = null;
            return false;
        }

        storage = mount.Storage;
        return true;
    }

    /// <summary>
    /// Tests whether a given relative path is a mount point.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><see langword="true"/> if the path is mounted. <see langword="false"/> otherwise.</returns>
    public bool IsMounted([StringSyntax(StringSyntaxAttribute.Uri)] string path)
    {
        return storages.ContainsKey(makeRelativePath(path));
    }

    public override bool CreateDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return false;
        }

        if (!mount.Writable)
        {
            throw readOnlyMountException;
        }

        return mount.Storage.CreateDirectory(subPath);
    }

    public override bool Delete(string path)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return false;
        }

        if (!mount.Writable)
        {
            throw readOnlyMountException;
        }

        return mount.Storage.Delete(subPath);
    }

    public override bool DeleteDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return false;
        }

        if (!mount.Writable)
        {
            throw readOnlyMountException;
        }

        return mount.Storage.DeleteDirectory(subPath);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return Enumerable.Empty<string>();
        }

        return mount.Storage.EnumerateDirectories(subPath, pattern, options);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return Enumerable.Empty<string>();
        }

        return mount.Storage.EnumerateFiles(subPath, pattern, options);
    }

    public override bool Exists(string path)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return false;
        }

        return mount.Storage.Exists(subPath);
    }

    public override bool ExistsDirectory(string path)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            return false;
        }

        return mount.Storage.ExistsDirectory(subPath);
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!resolve(path, out string? subPath, out var mount))
        {
            throw new DirectoryNotFoundException();
        }

        if ((access & FileAccess.Write) != 0 && !mount.Writable)
        {
            throw readOnlyMountException;
        }

        return mount.Storage.Open(subPath, mode, access);
    }

    private bool resolve(string path, [NotNullWhen(true)] out string? subPath, out MountedStorage storage)
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

        storages.Clear();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    private static string makeRelativePath(string path)
    {
        if (!(path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)))
        {
            path += Path.AltDirectorySeparatorChar;
        }

        return new Uri(baseUri, path).AbsolutePath;
    }

    private static readonly Uri baseUri = new("file:///");
    private static readonly Exception readOnlyMountException = new InvalidOperationException("Cannot write to a read-only mount.");

    private readonly record struct MountedStorage(Storage Storage, bool Writable);
}
