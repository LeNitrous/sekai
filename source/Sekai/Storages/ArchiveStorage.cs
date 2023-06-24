// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> backed by a <see cref="ZipArchive"/>.
/// </summary>
/// <remarks>
/// This type of storage does not support any directory-based operations.
/// </remarks>
public sealed class ArchiveStorage : Storage
{
    private bool isDisposed;
    private readonly bool leaveOpen;
    private readonly ZipArchive archive;

    public ArchiveStorage(string path, ZipArchiveMode mode = ZipArchiveMode.Read)
        : this(File.OpenRead(path), mode)
    {

    }

    public ArchiveStorage(Stream stream, ZipArchiveMode mode = ZipArchiveMode.Read)
        : this(new ZipArchive(stream, mode, false))
    {
    }

    public ArchiveStorage(ZipArchive archive, bool leaveOpen = false)
    {
        this.archive = archive;
        this.leaveOpen = leaveOpen;
    }

    public override bool CreateDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool Delete([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool DeleteDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override IEnumerable<string> EnumerateDirectories([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return Enumerable.Empty<string>();
    }

    public override IEnumerable<string> EnumerateFiles([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return archive.Entries.Select(e => e.FullName);
    }

    public override bool Exists([StringSyntax("Uri")] string path)
    {
        return EnumerateFiles(Path.GetDirectoryName(path) ?? string.Empty).Contains(path);
    }

    public override bool ExistsDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override Stream Open([StringSyntax("Uri")] string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        Stream stream = null!;

        bool exists = Exists(path);

        if (access is FileAccess.Read && archive.Mode is not ZipArchiveMode.Read or ZipArchiveMode.Update)
        {
            throw new ArgumentException("Cannot read from a write-only archive.", nameof(access));
        }

        if (access is FileAccess.Write && archive.Mode is not ZipArchiveMode.Create or ZipArchiveMode.Update)
        {
            throw new ArgumentException("Cannot write from a read-only archive.", nameof(access));
        }

        if (access is FileAccess.ReadWrite && archive.Mode is not ZipArchiveMode.Update)
        {
            throw new ArgumentException("Archive does not allow read and write operations.", nameof(access));
        }

        switch (mode)
        {
            case FileMode.OpenOrCreate when !exists:
            case FileMode.CreateNew when !exists:
            case FileMode.Append when !exists:
            case FileMode.Create when !exists:
                {
                    stream = archive.CreateEntry(Path.GetFileName(path)).Open();
                    break;
                }

            case FileMode.Truncate when exists:
            case FileMode.Create when exists:
                {
                    stream = archive.GetEntry(path)!.Open();
                    stream.SetLength(0);
                    break;
                }

            case FileMode.CreateNew when exists:
                throw new IOException($"File {path} already exists");

            case FileMode.Append when access == FileAccess.Write:
                throw new ArgumentException($"{nameof(FileMode.Append)} can only be used with {nameof(FileAccess.Read)}.");

            case FileMode.Open when !exists:
                throw new FileNotFoundException($"File {path} does not exist.", nameof(path));
        }

        return new WrappedStream(stream, mode == FileMode.Append);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        if (!leaveOpen)
        {
            archive.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
