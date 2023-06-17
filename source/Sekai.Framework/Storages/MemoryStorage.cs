// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Sekai.Framework.Storages;

public sealed class MemoryStorage : Storage
{
    private bool isDisposed;
    private readonly Dictionary<string, MemoryStream> files = new();
    private readonly Dictionary<string, MemoryStorage> storages = new();

    public override bool CreateDirectory(string path)
    {
        if (storages.ContainsKey(path))
        {
            return false;
        }

        storages.Add(path, new MemoryStorage());

        return true;
    }

    public override bool Delete(string path)
    {
        if (files.Remove(path, out var stream))
        {
            stream.Dispose();
            return true;
        }

        return false;
    }

    public override bool DeleteDirectory(string path)
    {
        if (storages.Remove(path, out var storage))
        {
            storage.Dispose();
            return true;
        }

        return false;
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        return matcher.Match(storages.Keys).Files.Select(f => f.Path);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        return matcher.Match(files.Keys).Files.Select(f => f.Path);
    }

    public override bool Exists(string path)
    {
        return files.ContainsKey(path);
    }

    public override bool ExistsDirectory(string path)
    {
        return storages.ContainsKey(path);
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!files.TryGetValue(path, out var value))
        {
            value = new MemoryStream();
        }

        var stream = new WrappedStream(value);

        return stream;
    }


    ~MemoryStorage()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        foreach (var file in files.Values)
        {
            file.Dispose();
        }

        foreach (var directory in storages.Values)
        {
            directory.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    private sealed class WrappedStream : Stream
    {
        public override bool CanRead => stream.CanRead;
        public override bool CanSeek => stream.CanSeek;
        public override bool CanWrite => stream.CanWrite;
        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        private readonly MemoryStream source;
        private readonly MemoryStream stream = new();

        public WrappedStream(MemoryStream source)
        {
            this.source = source;
            this.source.WriteTo(stream);
            this.source.Position = 0;
        }

        public override void Flush()
        {
            stream.Flush();
            stream.WriteTo(source);
            source.Position = 0;
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
            base.Dispose(disposing);

            if (disposing)
            {
                stream.Dispose();
            }
        }
    }
}
