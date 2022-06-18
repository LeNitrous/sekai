// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sekai.Framework.Storage;

public class InMemoryStorage : IStorage
{
    private readonly Dictionary<string, byte[]> files = new();
    private static readonly string root = "/";

    public bool CreateDirectory(string path)
    {
        throw new NotSupportedException();
    }

    public void Delete(string path)
    {
        path = localizePath(path);

        if (!files.ContainsKey(path))
            throw new FileNotFoundException(null, Path.GetFileName(path));

        files.Remove(path);
    }

    public void DeleteDirectory(string path)
    {
        throw new NotSupportedException();
    }

    public IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        throw new NotSupportedException();
    }

    public IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
        return files.Keys.Select(localizePath);
    }

    public bool Exists(string path)
    {
        return files.ContainsKey(localizePath(path));
    }

    public bool ExistsDirectory(string path)
    {
        throw new NotSupportedException();
    }

    public Stream? Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        path = localizePath(path);

        return Exists(path) && mode == FileMode.CreateNew
            ? throw new InvalidOperationException()
            : !Exists(path) && mode == FileMode.Open
            ? throw new FileNotFoundException(null, Path.GetFileName(path))
            : (Stream)new InMemoryStorageStream(this, path);
    }

    private static string localizePath(string path)
    {
        return Path.Combine(root, path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private class InMemoryStorageStream : Stream
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

        private readonly string path;
        private readonly MemoryStream stream;
        private readonly InMemoryStorage storage;

        public InMemoryStorageStream(InMemoryStorage storage, string path)
        {
            this.path = path;
            this.storage = storage;

            stream = new MemoryStream();

            if (storage.files.ContainsKey(path) && storage.files[path] != null)
            {
                stream.Write(storage.files[path]);
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
    }
}
