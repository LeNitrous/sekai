// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;

namespace Sekai.Storages;

/// <summary>
/// Storaged backed by the user's file system.
/// </summary>
public class NativeStorage : Storage
{
    public readonly string LocalBasePath;

    public NativeStorage(string localBasePath)
    {
        LocalBasePath = localBasePath;

        if (!Directory.Exists(localBasePath))
            Directory.CreateDirectory(localBasePath);
    }

    public override bool CreateDirectory(string path)
    {
        try
        {
            Directory.CreateDirectory(getFullPath(path));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override void Delete(string path)
    {
        File.Delete(getFullPath(path));
    }

    public override void DeleteDirectory(string path)
    {
        Directory.Delete(getFullPath(path));
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        return Directory.EnumerateDirectories(getFullPath(path), pattern);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateFiles(getFullPath(path), pattern, searchOption);
    }

    public override bool Exists(string path)
    {
        return File.Exists(getFullPath(path));
    }

    public override bool ExistsDirectory(string path)
    {
        return Directory.Exists(getFullPath(path));
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return File.Open(getFullPath(path), mode, access);
    }

    private string getFullPath(string path)
    {
        return Path.Combine(LocalBasePath, path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
