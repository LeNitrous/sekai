// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using DotNet.Globbing;

namespace Sekai.Framework.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> whose contents live in-memory.
/// </summary>
public sealed class MemoryStorage : Storage
{
    private const string storage_uri = "file:///";

    private bool isDisposed;
    private readonly Uri baseUri = new(storage_uri, UriKind.Absolute);
    private readonly Dictionary<string, MemoryStream> files = new();
    private readonly Dictionary<string, MemoryStorage> storages = new();

    public override bool CreateDirectory(string path)
    {
        string uri = createPath(baseUri, path);

        if (storages.ContainsKey(uri))
        {
            return false;
        }

        storages.Add(uri, new MemoryStorage());

        return true;
    }

    public override bool Delete(string path)
    {
        string uri = createPath(baseUri, path);

        if (files.Remove(uri, out var stream))
        {
            stream.Dispose();
            return true;
        }

        return false;
    }

    public override bool DeleteDirectory(string path)
    {
        string uri = createPath(baseUri, path);

        if (storages.Remove(uri, out var storage))
        {
            storage.Dispose();
            return true;
        }

        return false;
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        var glob = Glob.Parse(pattern);

        foreach (string storage in storages.Keys)
        {
            if (!glob.IsMatch(path))
            {
                continue;
            }

            yield return storage;
        }
    }

    public override IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        var glob = Glob.Parse(pattern);

        foreach (string file in files.Keys)
        {
            if (!glob.IsMatch(path))
            {
                continue;
            }

            yield return file;
        }
    }

    public override bool Exists(string path)
    {
        return files.ContainsKey(createPath(baseUri, path));
    }

    public override bool ExistsDirectory(string path)
    {
        return storages.ContainsKey(createPath(baseUri, path));
    }

    public override Stream Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        string uri = createPath(baseUri, path);

        if (!files.TryGetValue(uri, out var value))
        {
            value = new MemoryStream();
            files.Add(uri, value);
        }

        var stream = new WrappedStream(value);

        return stream;
    }

    private static string createPath(Uri baseUri, string path)
    {
        if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var relative))
        {
            throw new ArgumentException("Path has invalid characters.", nameof(path));
        }

        string absolutePath = new Uri(baseUri, relative).AbsolutePath;

        if (separator != Path.DirectorySeparatorChar)
        {
            absolutePath = absolutePath.Replace(Path.DirectorySeparatorChar, separator);
        }

        return absolutePath;
    }

    private const char separator = '/';

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
}
