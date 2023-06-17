// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Sekai.Framework.Storages;

/// <summary>
/// Access to storage-related operations such as retrieving and manipulating files and directories.
/// </summary>
public abstract class Storage : IDisposable
{
    /// <summary>
    /// Opens a file as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="path">The path to the stream.</param>
    /// <param name="mode">The mode of opening.</param>
    /// <param name="access">The access intent.</param>
    /// <returns>A stream to the opened file.</returns>
    public abstract Stream Open([StringSyntax(StringSyntaxAttribute.Uri)] string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite);

    /// <summary>
    /// Gets whether a file exists on a given path.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns><see langword="true"/> if the file exists. Otherwise <see langword="false"/></returns>
    public abstract bool Exists([StringSyntax(StringSyntaxAttribute.Uri)] string path);

    /// <summary>
    /// Deletes a file on a given path.
    /// </summary>
    /// <param name="path">The file to delete.</param>
    /// <returns><see langword="true"/> if the file has been deleted. Otherwise <see langword="false"/></returns>
    public abstract bool Delete([StringSyntax(StringSyntaxAttribute.Uri)] string path);

    /// <summary>
    /// Opens a directory as a <see cref="Storage"/>.
    /// </summary>
    /// <param name="path">The path to the directory.</param>
    /// <returns>Storage of the opened directory.</returns>
    public Storage OpenDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path) => Exists(path) ? new SubDirectoryStorage(this, path) : throw new FileNotFoundException(null, Path.GetFileName(path));

    /// <summary>
    /// Gets whether a directory exists on a given path.
    /// </summary>
    /// <param name="path">The directory to check.</param>
    /// <returns><see langword="true"/> if the directory exists. Otherwise <see langword="false"/></returns>
    public abstract bool ExistsDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path);

    /// <summary>
    /// Deletes a directory on a given path.
    /// </summary>
    /// <param name="path">The directory to delete.</param>
    /// <returns><see langword="true"/> if the directory has been deleted. Otherwise <see langword="false"/></returns>
    public abstract bool DeleteDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path);

    /// <summary>
    /// Creates a directory on a given path.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    /// <returns><see langword="true"/> if the directory has been create. Otherwise <see langword="false"/></returns>
    public abstract bool CreateDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path);

    /// <summary>
    /// Enumerates files on a given path.
    /// </summary>
    /// <param name="path">The path to enumerate from.</param>
    /// <param name="pattern">The search pattern.</param>
    /// <param name="options">The search options.</param>
    /// <returns>An enumeration of file paths on the given path.</returns>
    public abstract IEnumerable<string> EnumerateFiles([StringSyntax(StringSyntaxAttribute.Uri)] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Enumerates directories on a given path.
    /// </summary>
    /// <param name="path">The path to enumerate from.</param>
    /// <param name="pattern">The search pattern.</param>
    /// <param name="options">The search options.</param>
    /// <returns>An enumeration of directories paths on the given path.</returns>
    public abstract IEnumerable<string> EnumerateDirectories([StringSyntax(StringSyntaxAttribute.Uri)] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

    public abstract void Dispose();

    private sealed class SubDirectoryStorage : Storage
    {
        private readonly string basePath;
        private readonly Storage storage;

        public SubDirectoryStorage(Storage storage, string basePath)
        {
            this.storage = storage;
            this.basePath = basePath;
        }

        public override bool CreateDirectory(string path)
        {
            return storage.CreateDirectory(Path.Combine(basePath, path));
        }

        public override bool Delete(string path)
        {
            return storage.Delete(Path.Combine(basePath, path));
        }

        public override bool DeleteDirectory(string path)
        {
            return storage.DeleteDirectory(Path.Combine(basePath, path));
        }

        public override bool Exists(string path)
        {
            return storage.Exists(Path.Combine(basePath, path));
        }

        public override bool ExistsDirectory(string path)
        {
            return storage.ExistsDirectory(Path.Combine(basePath, path));
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return storage.EnumerateDirectories(Path.Combine(basePath, path), pattern, options);
        }

        public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return storage.EnumerateFiles(Path.Combine(basePath, path), pattern, options);
        }

        public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
        {
            return storage.Open(Path.Combine(basePath, path), mode, access);
        }

        public override void Dispose()
        {
        }
    }
}
