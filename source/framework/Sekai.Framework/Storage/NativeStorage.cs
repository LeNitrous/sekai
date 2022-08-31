// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;

namespace Sekai.Framework.Storage;

/// <summary>
/// Storaged backed by the user's file system.
/// </summary>
public class NativeStorage : IStorage
{
    public readonly string LocalBasePath;

    public NativeStorage(string localBasePath)
    {
        LocalBasePath = localBasePath;

        if (!Directory.Exists(localBasePath))
            Directory.CreateDirectory(localBasePath);
    }

    public bool CreateDirectory(string path)
    {
        try
        {
            Directory.CreateDirectory(GetFullPath(path));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Delete(string path)
    {
        File.Delete(GetFullPath(path));
    }

    public void DeleteDirectory(string path)
    {
        Directory.Delete(GetFullPath(path));
    }

    public IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        return Directory.EnumerateDirectories(GetFullPath(path), pattern);
    }

    public IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateFiles(GetFullPath(path), pattern, searchOption);
    }

    public bool Exists(string path)
    {
        return File.Exists(GetFullPath(path));
    }

    public bool ExistsDirectory(string path)
    {
        return Directory.Exists(GetFullPath(path));
    }

    public Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return File.Open(GetFullPath(path), mode, access);
    }

    public string GetFullPath(string path)
    {
        return Path.Combine(LocalBasePath, path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
