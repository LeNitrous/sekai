// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// Storage backed by a <see cref="ZipArchive"/>.
/// </summary>
public class ArchiveStorage : Storage
{
    public override bool CanRead => archive.Mode != ZipArchiveMode.Create;
    public override bool CanWrite => archive.Mode != ZipArchiveMode.Read;

    private readonly ZipArchive archive;

    public ArchiveStorage(Uri path, ZipArchiveMode mode = ZipArchiveMode.Read)
        : this(File.OpenRead(path.AbsolutePath), mode)
    {
    }

    public ArchiveStorage(Stream stream, ZipArchiveMode mode = ZipArchiveMode.Read)
        : this(new ZipArchive(stream, mode))
    {
    }

    public ArchiveStorage(ZipArchive archive)
    {
        this.archive = archive;
    }

    protected override bool BaseCreateDirectory(Uri uri)
    {
        return false;
    }

    protected override bool BaseDelete(Uri uri)
    {
        return false;
    }

    protected override bool BaseDeleteDirectory(Uri uri)
    {
        return false;
    }

    protected override IEnumerable<Uri> BaseEnumerateDirectories(Uri uri)
    {
        return Enumerable.Empty<Uri>();
    }

    protected override IEnumerable<Uri> BaseEnumerateFiles(Uri uri)
    {
        return archive.Entries.Select(e => new Uri(Uri, new Uri(e.FullName))).Where(uri.IsBaseOf);
    }

    protected override bool BaseExists(Uri uri)
    {
        return EnumerateFiles(uri).Contains(uri);
    }

    protected override bool BaseExistsDirectory(Uri uri)
    {
        return false;
    }

    protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        Stream stream = null!;

        switch (mode)
        {
            case FileMode.OpenOrCreate when !Exists(uri):
            case FileMode.CreateNew when !Exists(uri):
            case FileMode.Append when !Exists(uri):
            case FileMode.Create when !Exists(uri):
                {
                    stream = archive.CreateEntry(Path.GetFileName(uri.AbsolutePath)).Open();
                    break;
                }

            case FileMode.Truncate when Exists(uri):
            case FileMode.Create when Exists(uri):
                {
                    stream = archive.GetEntry(uri.AbsolutePath)!.Open();
                    stream.SetLength(0);
                    break;
                }

            case FileMode.CreateNew when Exists(uri):
                throw new IOException($"File {uri} already exists");

            case FileMode.Append when access == FileAccess.Write:
                throw new ArgumentException($"{nameof(FileMode.Append)} can only be used with {nameof(FileAccess.Read)}.");

            case FileMode.Open when !Exists(uri):
                throw new FileNotFoundException($"File {uri} does not exist.", nameof(uri));
        }

        var memory = new MemoryStream();

        stream.CopyTo(memory);

        if (mode != FileMode.Append)
            memory.Position = 0;

        return memory;
    }

    protected override void Destroy() => archive.Dispose();

}
