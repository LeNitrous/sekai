// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;

namespace Sekai.Storages;

/// <summary>
/// Base class for all storages.
/// </summary>
public abstract class Storage : FrameworkObject
{
    /// <summary>
    /// Opens a file given its file path as a stream.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="mode">The mode of access to the file.</param>
    /// <param name="access">The nature of access to the file.</param>
    public abstract Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite);

    /// <summary>
    /// Gets whether a file exists on the given path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    public abstract bool Exists(string path);

    /// <summary>
    /// Deletes a file on the given path.
    /// </summary>
    /// <param name="path">The file to be deleted.</param>
    public abstract void Delete(string path);

    /// <summary>
    /// Enumerates all files on the given path.
    /// </summary>
    /// <param name="path">The path to enumerate files.</param>
    /// <param name="pattern">The search pattern to use for filtering results.</param>
    /// <param name="searchOptions">The search options to use to filter results.</param>
    public abstract IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Gets whether a directory exists on a given path.
    /// </summary>
    /// <param name="path">A path to the directory.</param>
    public abstract bool ExistsDirectory(string path);

    /// <summary>
    /// Creates a directory on the given path.
    /// </summary>
    /// <param name="path">The path where the directory will be created.</param>
    public abstract bool CreateDirectory(string path);

    /// <summary>
    /// Deletes a directory on the given path.
    /// </summary>
    /// <param name="path">The path where the directory will be deleted.</param>
    public abstract void DeleteDirectory(string path);

    /// <summary>
    /// Enumerates all directories on a given path.
    /// </summary>
    /// <param name="path">The path to enumerate directories.</param>
    /// <param name="pattern">The search pattern to use for filtering results</param>
    public abstract IEnumerable<string> EnumerateDirectories(string path, string pattern = "*");

    /// <summary>
    /// Gets a <see cref="Storage"/> for a given sub directory.
    /// </summary>
    /// <param name="path">The directory's path relative to this storage.</param>
    /// <param name="createIfNotExist">Create the directory if it doesn't exist.</param>
    /// <returns>A storage for the given directory</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    public Storage GetStorageForDirectory(string path, bool createIfNotExist = false)
    {
        if (!ExistsDirectory(path))
        {
            if (!createIfNotExist)
                throw new DirectoryNotFoundException();

            CreateDirectory(path);
        }

        return new SubPathStorage(this, path);
    }

    private class SubPathStorage : Storage
    {
        private readonly string path;
        private readonly Storage parent;

        public SubPathStorage(Storage parent, string path)
        {
            this.path = path;
            this.parent = parent;
        }

        public override bool CreateDirectory(string path)
        {
            return parent.CreateDirectory(getFullPath(path));
        }

        public override void Delete(string path)
        {
            parent.Delete(getFullPath(path));
        }

        public override void DeleteDirectory(string path)
        {
            parent.DeleteDirectory(getFullPath(path));
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
        {
            return parent.EnumerateDirectories(getFullPath(path), pattern);
        }

        public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return parent.EnumerateFiles(getFullPath(path));
        }

        public override bool Exists(string path)
        {
            return parent.Exists(getFullPath(path));
        }

        public override bool ExistsDirectory(string path)
        {
            return parent.ExistsDirectory(getFullPath(path));
        }

        public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
        {
            return parent.Open(getFullPath(path), mode, access);
        }

        private string getFullPath(string path)
        {
            return Path.Combine(this.path, path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
