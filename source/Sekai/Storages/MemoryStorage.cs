// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using Sekai.Collections;

namespace Sekai.Storages;

/// <summary>
/// Storage backed by memory.
/// </summary>
public class MemoryStorage : Storage
{
    private readonly Dictionary<Uri, Stream> streams = new(UriEqualityComparer.Default);
    private readonly Dictionary<Uri, Storage> storages = new(UriEqualityComparer.Default);

    protected override bool BaseCreateDirectory(Uri path)
    {
        if (storages.ContainsKey(path))
            return false;

        storages.Add(path, new MemoryStorage());

        return true;
    }

    protected override bool BaseDelete(Uri path)
    {
        if (!streams.Remove(path, out var stream))
            return false;

        stream.Dispose();

        return true;
    }

    protected override bool BaseDeleteDirectory(Uri path)
    {
        if (!storages.Remove(path, out var storage))
            return false;

        storage.Dispose();

        return true;
    }

    protected override IEnumerable<Uri> BaseEnumerateDirectories(Uri path)
    {
        return storages.Keys;
    }

    protected override IEnumerable<Uri> BaseEnumerateFiles(Uri path)
    {
        return streams.Keys;
    }

    protected override bool BaseExists(Uri path)
    {
        return streams.ContainsKey(path);
    }

    protected override bool BaseExistsDirectory(Uri path)
    {
        return storages.ContainsKey(path);
    }

    protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        var memory = new MemoryStream();
        var stream = streams[uri];

        switch (mode)
        {
            case FileMode.OpenOrCreate when !Exists(uri):
            case FileMode.CreateNew when !Exists(uri):
            case FileMode.Append when !Exists(uri):
            case FileMode.Create when !Exists(uri):
                {
                    streams[uri] = stream = new MemoryStream();
                    break;
                }

            case FileMode.Truncate when Exists(uri):
            case FileMode.Create when Exists(uri):
                {
                    stream.SetLength(0);
                    break;
                }

            case FileMode.CreateNew when Exists(uri):
                throw new IOException($"File {uri} already exists");

            case FileMode.Append when access == FileAccess.Write:
                throw new ArgumentException($"{nameof(FileMode.Append)} can only be used with {nameof(FileAccess.Read)}.");

            case FileMode.Truncate when !Exists(uri):
            case FileMode.Open when !Exists(uri):
                throw new FileNotFoundException($"File {uri} does not exist.", nameof(uri));
        }

        stream.CopyTo(memory);   

        if (mode != FileMode.Append)
            memory.Position = 0;

        return new MemoryStorageStream(memory, mode, access);
    }

    protected override void Destroy()
    {
        foreach (var stream in streams.Values)
            stream.Dispose();

        foreach (var storage in storages.Values)
            storage.Dispose();

        streams.Clear();
        storages.Clear();
    }

    private class MemoryStorageStream : Stream
    {
        public override bool CanRead => access != FileAccess.Write;

        public override bool CanSeek => true;

        public override bool CanWrite => access != FileAccess.Read;

        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set
            {
                if (mode == FileMode.Append && value < stream.Length)
                    throw new NotSupportedException($"Attempted to seek behind stream length while mode is {nameof(FileMode.Append)}.");

                stream.Position = value;
            }
        }

        private readonly Stream stream;
        private readonly FileMode mode;
        private readonly FileAccess access;

        public MemoryStorageStream(Stream stream, FileMode mode, FileAccess access)
        {
            this.mode = mode;
            this.access = access;
            this.stream = stream;
        }

        public override void Flush()
        {
            if (!CanWrite)
                throw new NotSupportedException("Cannot set length of a read-only stream.");

            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (mode == FileMode.Truncate)
                throw new ArgumentException("Attempted to read from a truncated stream.");

            if (mode == FileMode.Append || !CanRead)
                throw new NotSupportedException("Cannot read from a write-only stream.");

            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long start = origin switch
            {
                SeekOrigin.Begin => 0,
                SeekOrigin.Current => Position,
                SeekOrigin.End => Length,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            return Position += start + offset;
        }

        public override void SetLength(long value)
        {
            if (!CanWrite)
                throw new NotSupportedException("Cannot set length of a read-only stream.");

            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new NotSupportedException("Cannot write to a read-only stream.");

            stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            stream.Dispose();
        }
    }
}
