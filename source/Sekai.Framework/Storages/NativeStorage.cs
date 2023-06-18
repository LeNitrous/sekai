// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;

namespace Sekai.Framework.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> that uses <see cref="File"/> and <see cref="Directory"/> operations.
/// </summary>
public sealed class NativeStorage : Storage
{
    private readonly DirectoryInfo directory;

    public NativeStorage(DirectoryInfo directory)
    {
        this.directory = directory;
    }

    public NativeStorage(string path)
        : this(new DirectoryInfo(path))
    {
    }

    public override bool CreateDirectory(string path)
    {
        try
        {
            directory.CreateSubdirectory(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override bool Delete(string path)
    {
        try
        {
            File.Delete(Path.Combine(directory.FullName, path));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override bool DeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(Path.Combine(directory.FullName, path), true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override bool Exists(string path)
    {
        return File.Exists(Path.Combine(directory.FullName, path));
    }

    public override bool ExistsDirectory(string path)
    {
        return Directory.Exists(Path.Combine(directory.FullName, path));
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateDirectories(Path.Combine(directory.FullName, path), pattern, options);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateFiles(Path.Combine(directory.FullName, path), pattern, options);
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return File.Open(Path.Combine(directory.FullName, path), mode, access);
    }

    public override void Dispose()
    {
    }
}
