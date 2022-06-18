// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DotNet.Globbing;

namespace Sekai.Framework.Storage;

public class ArchiveBackedStorage : FrameworkObject, IStorage
{
    private readonly ZipArchive archive;
    private readonly IEnumerable<string> entries;

    public ArchiveBackedStorage(Stream archiveStream)
    {
        archive = new ZipArchive(archiveStream, ZipArchiveMode.Read);
        entries = archive.Entries.Select(e => e.FullName);
    }

    public ArchiveBackedStorage(string path)
        : this(File.OpenRead(path))
    {
    }

    public bool CreateDirectory(string path)
    {
        throw new NotSupportedException(@"Cannot create directories in an archive.");
    }

    public void Delete(string path)
    {
        throw new UnauthorizedAccessException(@"Archives are read-only.");
    }

    public void DeleteDirectory(string path)
    {
        throw new NotSupportedException(@"Cannot delete directories in an archive.");
    }

    public IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        throw new NotSupportedException(@"Cannot enumerate directories in an archive.");
    }

    public IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
        var glob = Glob.Parse(pattern);
        return entries.Where(e => glob.IsMatch(e));
    }

    public bool Exists(string path)
    {
        return entries.Contains(path);
    }

    public bool ExistsDirectory(string path)
    {
        throw new NotSupportedException(@"Cannot enumerate directories in an archive.");
    }

    public Stream? Open(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
    {
        if (!Exists(path))
            throw new FileNotFoundException(null, path);

        if (mode != FileMode.Open || access != FileAccess.Read)
            throw new UnauthorizedAccessException(@"Archives are read-only");

        lock (archive)
        {
            var entry = archive.GetEntry(path);
            var readable = entry?.Open();

            if (readable == null)
                return null;

            var writable = new MemoryStream();

            readable.CopyTo(writable);
            writable.Position = 0;

            return writable;
        }
    }

    protected override void Destroy()
    {
        archive.Dispose();
    }
}
