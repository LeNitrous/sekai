// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Sekai.Storages;

/// <summary>
/// A virtual storage which can mount other storages and interact using virtual URI paths.
/// </summary>
public class VirtualStorage : Storage
{
    private readonly Dictionary<Uri, Storage> storages = new();

    /// <inheritdoc cref="Mount(Uri, Storage)"/>
    /// <param name="path">A relative path where the storage will be mounted to.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is not a relative path.</exception>
    public void Mount(string path, Storage storage)
    {
        path = path[^1] != Path.AltDirectorySeparatorChar ? path + Path.AltDirectorySeparatorChar : path;

        if (!Uri.IsWellFormedUriString(path, UriKind.Relative))
            throw new ArgumentException("Path must be a relative path.", nameof(path));

        var uri = new Uri(path, UriKind.RelativeOrAbsolute);

        if (uri.IsAbsoluteUri && !Uri.IsBaseOf(uri))
            throw new ArgumentException("Path must be a relative path.");

        Mount(uri, storage);
    }

    /// <inheritdoc cref="Unmount(Uri)"/>
    public bool Unmount(string path)
    {
        path = path[^1] != Path.AltDirectorySeparatorChar ? path + Path.AltDirectorySeparatorChar : path;

        if (!Uri.IsWellFormedUriString(path, UriKind.Relative))
            throw new ArgumentException("Path must be a relative path.", nameof(path));

        var uri = new Uri(path, UriKind.RelativeOrAbsolute);

        if (uri.IsAbsoluteUri && !Uri.IsBaseOf(uri))
            throw new ArgumentException("Path must be a relative path.", nameof(path));

        return Unmount(uri);
    }

    /// <summary>
    /// Mounts a <see cref="Storage"/> at a given URI.
    /// </summary>
    /// <param name="uri">The URI where the storage will be mounted to.</param>
    /// <param name="storage">The storage to mount.</param>
    /// <exception cref="InvalidOperationException">Thrown when a storage is already mounted at a given URI.</exception>
    public void Mount(Uri uri, Storage storage)
    {
        if (uri.OriginalString[^1] != Path.AltDirectorySeparatorChar)
            throw new ArgumentException("URI must have a trailing slash.", nameof(uri));

        var actual = GetFullPath(uri);

        if (storages.ContainsKey(actual))
            throw new InvalidOperationException($"There is a storage already mounted at {uri}");

        storages[actual] = storage;
    }

    /// <summary>
    /// Unmounts a storage at a given URI.
    /// </summary>
    /// <param name="uri">The URI pointing to a storage to unmount from.</param>
    /// <param name="storage">The storage that was unmounted.</param>
    /// <returns>True if the storage was unmounted. False otherwise.</returns>
    public bool Unmount(Uri uri, [NotNullWhen(true)] out Storage? storage)
    {
        return storages.Remove(GetFullPath(uri), out storage);
    }

    /// <inheritdoc cref="Unmount(Uri, out Storage?)"/>
    public bool Unmount(Uri uri)
    {
        return Unmount(uri, out _);
    }

    protected override bool BaseCreateDirectory(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.CreateDirectory(relativeUri);
    }

    protected override bool BaseDelete(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.Delete(relativeUri);
    }

    protected override bool BaseDeleteDirectory(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.DeleteDirectory(relativeUri);
    }

    protected override IEnumerable<Uri> BaseEnumerateDirectories(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.EnumerateDirectories(relativeUri);
    }

    protected override IEnumerable<Uri> BaseEnumerateFiles(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.EnumerateFiles(relativeUri);
    }

    protected override bool BaseExists(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.Exists(relativeUri);
    }

    protected override bool BaseExistsDirectory(Uri uri)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.ExistsDirectory(relativeUri);
    }

    protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        var (relativeUri, storage) = resolve(uri);
        return storage.Open(relativeUri, mode, access);
    }

    private (Uri, Storage) resolve(Uri uri)
    {
        foreach (var entry in storages)
        {
            if (!entry.Key.IsBaseOf(uri))
                continue;

            return (new Uri(uri.AbsolutePath.Replace(entry.Key.AbsolutePath, string.Empty).TrimStart(Path.AltDirectorySeparatorChar), UriKind.Relative), entry.Value);
        }

        throw new DirectoryNotFoundException();
    }

    protected override void Destroy()
    {
        foreach (var storage in storages.Values)
            storage.Dispose();

        storages.Clear();
    }
}
