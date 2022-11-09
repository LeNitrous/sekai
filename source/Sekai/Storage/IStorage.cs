// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;

namespace Sekai.Storage;

/// <summary>
/// Interface for objects capable of performing storage-related actions.
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Opens a file given its file path as a stream.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="mode">The mode of access to the file.</param>
    /// <param name="access">The nature of access to the file.</param>
    Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite);

    /// <summary>
    /// Gets whether a file exists on the given path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    bool Exists(string path);

    /// <summary>
    /// Deletes a file on the given path.
    /// </summary>
    /// <param name="path">The file to be deleted.</param>
    void Delete(string path);

    /// <summary>
    /// Enumerates all files on the given path.
    /// </summary>
    /// <param name="path">The path to enumerate files.</param>
    /// <param name="pattern">The search pattern to use for filtering results.</param>
    /// <param name="searchOptions">The search options to use to filter results.</param>
    IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Gets whether a directory exists on a given path.
    /// </summary>
    /// <param name="path">A path to the directory.</param>
    bool ExistsDirectory(string path);

    /// <summary>
    /// Creates a directory on the given path.
    /// </summary>
    /// <param name="path">The path where the directory will be created.</param>
    bool CreateDirectory(string path);

    /// <summary>
    /// Deletes a directory on the given path.
    /// </summary>
    /// <param name="path">The path where the directory will be deleted.</param>
    void DeleteDirectory(string path);

    /// <summary>
    /// Enumerates all directories on a given path.
    /// </summary>
    /// <param name="path">The path to enumerate directories.</param>
    /// <param name="pattern">The search pattern to use for filtering results</param>
    IEnumerable<string> EnumerateDirectories(string path, string pattern = "*");
}
