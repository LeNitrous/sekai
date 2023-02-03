// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// Storage backed by physical media.
/// </summary>
public class NativeStorage : Storage
{
    public override Uri Uri { get; }

    public NativeStorage(Uri uri)
    {
        if (!uri.IsAbsoluteUri)
            throw new ArgumentException("URI must be absolute.", nameof(uri));

        Uri = uri;
    }

    protected override bool BaseCreateDirectory(Uri uri)
    {
        try
        {
            Directory.CreateDirectory(uri.AbsolutePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override bool BaseDelete(Uri uri)
    {
        try
        {
            File.Delete(uri.AbsolutePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override bool BaseDeleteDirectory(Uri uri)
    {
        try
        {
            Directory.Delete(uri.AbsolutePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<Uri> BaseEnumerateDirectories(Uri uri)
    {
        return Directory.EnumerateDirectories(uri.AbsolutePath).Select(p => new Uri(p));
    }

    protected override IEnumerable<Uri> BaseEnumerateFiles(Uri uri)
    {
        return Directory.EnumerateFiles(uri.AbsolutePath).Select(p => new Uri(p));
    }

    protected override bool BaseExists(Uri uri)
    {
        return File.Exists(uri.AbsolutePath);
    }

    protected override bool BaseExistsDirectory(Uri uri)
    {
        return Directory.Exists(uri.AbsolutePath);
    }

    protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return File.Open(uri.AbsolutePath, mode, access);
    }
}
