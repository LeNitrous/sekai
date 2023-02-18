// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Globbing;

namespace Sekai.Storages;

/// <summary>
/// Base class for all storages.
/// </summary>
public abstract class Storage : DisposableObject
{
    /// <summary>
    /// The storage's base URI.
    /// </summary>
    public virtual Uri Uri { get; } = new Uri(@"file:///", UriKind.Absolute);

    /// <summary>
    /// Gets whether this storage is readable or not.
    /// </summary>
    public virtual bool CanRead { get; } = true;

    /// <summary>
    /// Gets whether this storage is writable or not.
    /// </summary>
    public virtual bool CanWrite { get; } = true;

    /// <inheritdoc cref="Open(Uri, FileMode, FileAccess)"/>
    /// <param name="path">An absolute or relative path to the file.</param>
    public Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return Open(new Uri(path, UriKind.RelativeOrAbsolute), mode, access);
    }

    /// <inheritdoc cref="Exists(Uri)"/>
    /// <param name="path">An absolute or relative path to the file.</param>
    public bool Exists(string path)
    {
        return Exists(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc cref="Delete(Uri)"/>
    /// <param name="path">An absolute or relative path to the file.</param>
    public bool Delete(string path)
    {
        return Delete(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc cref="EnumerateFiles(Uri, string, SearchOption)"/>
    /// <param name="path">An absolute or relative path to the file.</param>
    public IEnumerable<Uri> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.AllDirectories)
    {
        return EnumerateFiles(new Uri(path, UriKind.RelativeOrAbsolute), pattern, options);
    }

    /// <inheritdoc cref="ExistsDirectory(Uri)"/>
    /// <param name="path">An absolute or relative path to the directory.</param>
    public bool ExistsDirectory(string path)
    {
        return ExistsDirectory(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc cref="DeleteDirectory(Uri)"/>
    /// <param name="path">An absolute or relative path to the directory.</param>
    public bool DeleteDirectory(string path)
    {
        return DeleteDirectory(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc cref="CreateDirectory(Uri)"/>
    /// <param name="path">An absolute or relative path to the directory.</param>
    public bool CreateDirectory(string path)
    {
        return CreateDirectory(new Uri(path, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc cref="EnumerateDirectories(Uri, string)"/>
    /// <param name="path">An absolute or relative path to the directory.</param>
    public IEnumerable<Uri> EnumerateDirectories(string path, string pattern = "*")
    {
        return EnumerateDirectories(new Uri(path, UriKind.RelativeOrAbsolute), pattern);
    }

    /// <inheritdoc cref="GetStorage"/>
    /// <param name="path">An absolute relative path to the storage.</param>
    public Storage GetStorage(string path)
    {
        path = path[^1] != Path.AltDirectorySeparatorChar ? path + Path.AltDirectorySeparatorChar : path;

        if (!Uri.IsWellFormedUriString(path, UriKind.Relative))
            throw new ArgumentException("Path must be a relative path.", nameof(path));

        var uri = new Uri(path, UriKind.RelativeOrAbsolute);

        if (uri.IsAbsoluteUri && !Uri.IsBaseOf(uri))
            throw new ArgumentException("Path must be a relative path.");

        return GetStorage(uri);
    }

    /// <summary>
    /// Opens a file as a stream from the given URI.
    /// </summary>
    /// <param name="uri">The URI to the file/</param>
    /// <param name="mode">The mode of access to the file.</param>
    /// <param name="access">The nature of access to the file.</param>
    /// <returns>The file stream.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when there are insufficent authorizations when attempting to open a file.</exception>
    public Stream Open(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!CanRead && access != FileAccess.Write)
            throw new UnauthorizedAccessException("Storage is not available for read access.");

        if (!CanWrite && access != FileAccess.Read)
            throw new UnauthorizedAccessException("Storage is not available for write access.");

        return BaseOpen(GetFullPath(uri), mode, access);
    }

    /// <summary>
    /// Determines whether a file exists on a given URI.
    /// </summary>
    /// <param name="uri">The URI to check from.</param>
    /// <returns>True if the file exists. False otherwise.</returns>
    public bool Exists(Uri uri)
    {
        return BaseExists(GetFullPath(uri));
    }

    /// <summary>
    /// Deletes a file on a given URI.
    /// </summary>
    /// <param name="uri">The URI to delete a file from.</param>
    /// <returns>True if the file was deleted. False otherwise.</returns>
    public bool Delete(Uri uri)
    {
        if (!CanWrite)
            return false;

        return BaseDelete(GetFullPath(uri));
    }

    /// <summary>
    /// Enumerates all files on a given directory.
    /// </summary>
    /// <param name="uri">The URI to enumerate files from.</param>
    /// <param name="pattern">A glob pattern that affects the result of the enumeration.</param>
    /// <param name="option">Search options that affect the result of the enumeration.</param>
    /// <returns>An enumeration of file URIs.</returns>
    public IEnumerable<Uri> EnumerateFiles(Uri uri, string pattern = "*", SearchOption option = SearchOption.AllDirectories)
    {
        var enumeration = BaseEnumerateFiles(GetFullPath(uri));

        if (pattern != patternWildcard)
            enumeration = enumeration.Where(p => Glob.Parse(pattern).IsMatch(p.AbsolutePath));

        if (option == SearchOption.TopDirectoryOnly)
            enumeration = enumeration.Where(p => Path.HasExtension(p.AbsolutePath));

        return enumeration;
    }

    /// <summary>
    /// Determines whether a directory exists on the given URI.
    /// </summary>
    /// <param name="uri">The URI to check from.</param>
    /// <returns>True if the directory exists. False otherwise.</returns>
    public bool ExistsDirectory(Uri uri)
    {
        return BaseExistsDirectory(GetFullPath(uri));
    }

    /// <summary>
    /// Deletes a directory on the given URI.
    /// </summary>
    /// <param name="uri">The URI to delete a directory from.</param>
    /// <returns>True if the directory was created. False otherwise.</returns>
    public bool DeleteDirectory(Uri uri)
    {
        return BaseDeleteDirectory(GetFullPath(uri));
    }

    /// <summary>
    /// Creates a directory on the given URI.
    /// </summary>
    /// <param name="uri">The URI to create a new directory from.</param>
    /// <returns>True if the directory was created. False otherwise.</returns>
    public bool CreateDirectory(Uri uri)
    {
        return BaseCreateDirectory(GetFullPath(uri));
    }

    /// <summary>
    /// Enumerates all directories on a given URI.
    /// </summary>
    /// <param name="uri">The URI to enumerate directories from.</param>
    /// <param name="pattern">The search pattern used to filter results.</param>
    /// <returns>An enumeration of directory URIs.</returns>
    public IEnumerable<Uri> EnumerateDirectories(Uri uri, string pattern = "*")
    {
        var enumeration = BaseEnumerateDirectories(GetFullPath(uri));

        if (pattern != patternWildcard)
            enumeration = enumeration.Where(p => Glob.Parse(pattern).IsMatch(p.AbsolutePath));

        return enumeration;
    }

    /// <summary>
    /// Creates a <see cref="Storage"/> for a relative URI.
    /// </summary>
    /// <param name="uri">A URI relative to this storage.</param>
    /// <returns>A new storage for the given directory.</returns>
    public Storage GetStorage(Uri uri)
    {
        if (uri.OriginalString[^1] != Path.AltDirectorySeparatorChar)
            throw new ArgumentException("URI must have a trailing slash.", nameof(uri));

        return CreateSubPathStorage(GetFullPath(uri));
    }

    /// <summary>
    /// Gets a well-formed absolute URI from a relative URI.
    /// </summary>
    /// <param name="uri">A relative URI.</param>
    /// <returns>An absolute URI that is rooted from <see cref="Uri"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="uri"/> is an absolute URI that is not rooted from <see cref="Uri"/>.</exception>
    public Uri GetFullPath(Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            if (!Uri.IsBaseOf(uri))
                throw new ArgumentException(@"URI is not relative to the base URI.", nameof(uri));

            return uri;
        }

        return new Uri(Uri, uri);
    }

    protected abstract Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite);

    protected abstract bool BaseExists(Uri uri);

    protected abstract bool BaseDelete(Uri uri);

    protected abstract IEnumerable<Uri> BaseEnumerateFiles(Uri uri);

    protected abstract bool BaseExistsDirectory(Uri uri);

    protected abstract bool BaseDeleteDirectory(Uri uri);

    protected abstract bool BaseCreateDirectory(Uri uri);

    protected abstract IEnumerable<Uri> BaseEnumerateDirectories(Uri uri);

    protected virtual Storage CreateSubPathStorage(Uri uri) => new SubPathStorage(this, uri);

    private static readonly string patternWildcard = "*";

    private class SubPathStorage : Storage
    {
        public override Uri Uri { get; }

        private readonly Storage storage;

        public SubPathStorage(Storage storage, Uri uri)
        {
            Uri = uri;
            this.storage = storage;
        }

        protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
        {
            return storage.BaseOpen(GetFullPath(uri), mode, access);
        }

        protected override bool BaseExists(Uri uri)
        {
            return storage.BaseExists(uri);
        }

        protected override bool BaseDelete(Uri uri)
        {
            return storage.BaseDelete(uri);
        }

        protected override IEnumerable<Uri> BaseEnumerateFiles(Uri uri)
        {
            return storage.BaseEnumerateFiles(uri);
        }

        protected override bool BaseExistsDirectory(Uri uri)
        {
            return storage.BaseExistsDirectory(uri);
        }

        protected override bool BaseDeleteDirectory(Uri uri)
        {
            return storage.BaseDeleteDirectory(uri);
        }

        protected override bool BaseCreateDirectory(Uri uri)
        {
            return storage.BaseCreateDirectory(uri);
        }

        protected override IEnumerable<Uri> BaseEnumerateDirectories(Uri uri)
        {
            return storage.BaseEnumerateDirectories(uri);
        }
    }
}
