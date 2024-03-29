// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> that uses <see cref="File"/> and <see cref="System.IO.Directory"/> operations.
/// </summary>
public class NativeStorage : Storage
{
    /// <summary>
    /// The directory where the native storage is located.
    /// </summary>
    protected DirectoryInfo Directory { get; }

    private bool isDisposed;

    public NativeStorage(DirectoryInfo directory)
    {
        Directory = directory;
        Directory.Create();
    }

    public NativeStorage(string path)
        : this(new DirectoryInfo(path))
    {
    }

    public override bool CreateDirectory(string path)
    {
        try
        {
            Directory.CreateSubdirectory(path);
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
            File.Delete(Path.Join(Directory.FullName, path));
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
            System.IO.Directory.Delete(Path.Join(Directory.FullName, path), true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override bool Exists(string path)
    {
        return File.Exists(Path.Join(Directory.FullName, path));
    }

    public override bool ExistsDirectory(string path)
    {
        return System.IO.Directory.Exists(Path.Join(Directory.FullName, path));
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return System.IO.Directory.EnumerateDirectories(Path.Join(Directory.FullName, path), pattern, options).Select(path => Path.AltDirectorySeparatorChar + path.Replace(Directory.FullName, string.Empty).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return System.IO.Directory.EnumerateFiles(Path.Join(Directory.FullName, path), pattern, options).Select(path => Path.AltDirectorySeparatorChar + path.Replace(Directory.FullName, string.Empty).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return File.Open(Path.Join(Directory.FullName, path), mode, access);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
