// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sekai.Storages;

/// <summary>
/// A non-persistent storage.
/// </summary>
public class MemoryStorage : Storage
{
    private readonly Dictionary<string, ReadOnlyMemory<byte>> files = new();

    public override bool CreateDirectory(string path)
    {
        throw new NotSupportedException();
    }

    public override void DeleteDirectory(string path)
    {
        throw new NotSupportedException();
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        throw new NotSupportedException();
    }

    public override bool ExistsDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public override void Delete(string path)
    {
        path = localizePath(path);

        if (!files.ContainsKey(path))
            throw new FileNotFoundException(null, Path.GetFileName(path));

        files.Remove(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
        return files.Keys.Select(localizePath);
    }

    public override bool Exists(string path)
    {
        return files.ContainsKey(localizePath(path));
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        path = localizePath(path);

        if (mode == FileMode.CreateNew && Exists(path))
            throw new IOException($"File \"{path}\" already exists.");

        if ((mode is FileMode.Open or FileMode.Create or FileMode.Truncate or FileMode.Append) && !Exists(path))
            throw new FileNotFoundException(null, Path.GetFileName(path));

        if (mode is FileMode.CreateNew or FileMode.OpenOrCreate or FileMode.Open)
            return new MemoryFileStream(this, path);

        if (mode == FileMode.Create)
        {
            if (Exists(path))
                Delete(path);

            return new MemoryFileStream(this, path);
        }

        if (mode == FileMode.Truncate)
        {
            var stream = new MemoryFileStream(this, path);
            stream.SetLength(0);
            return stream;
        }

        if (mode == FileMode.Append)
        {
            var stream = new MemoryFileStream(this, path);
            stream.Seek(stream.Length, SeekOrigin.Begin);
            return stream;
        }

        throw new InvalidOperationException($"{mode} is not a valid file mode.");
    }

    private static string localizePath(string path)
    {
        return Path.Combine(Path.DirectorySeparatorChar.ToString(), path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private class MemoryFileStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        private readonly string path;
        private readonly MemoryStream stream;
        private readonly MemoryStorage storage;

        public MemoryFileStream(MemoryStorage storage, string path)
        {
            this.path = path;
            this.storage = storage;

            stream = new MemoryStream();

            if (storage.files.TryGetValue(path, out var value))
            {
                stream.Write(value.Span);
                stream.Position = 0;
            }
        }

        public override void Flush()
        {
            if (storage.files.ContainsKey(path))
            {
                storage.files[path] = stream.ToArray();
            }
            else
            {
                storage.files.Add(path, stream.ToArray());
            }

            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            stream.Dispose();
        }
    }
}
