// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sekai.Framework.Storage;

public class VirtualStorage : FrameworkObject, IStorage
{
    private readonly Dictionary<string, IStorage> storages = new();

    public VirtualStorage(IStorage? root = null)
    {
        if (root != null)
            Mount("/", root);
    }

    public void Mount(string basePath, IStorage storage)
    {
        if (string.IsNullOrEmpty(basePath))
            throw new ArgumentException("Path cannot be null or empty", nameof(basePath));

        if (storages.ContainsKey(basePath))
            throw new InvalidOperationException();

        basePath = basePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (!basePath.StartsWith(Path.AltDirectorySeparatorChar))
            basePath = Path.AltDirectorySeparatorChar + basePath;

        if (!basePath.EndsWith(Path.AltDirectorySeparatorChar))
            basePath += Path.AltDirectorySeparatorChar;

        storages.Add(basePath, storage);
    }

    public void Unmount(string basePath)
    {
        storages.Remove(basePath);
    }

    public IStorage GetStorage(string path)
    {
        var result = resolve(path);
        return result.IStorage;
    }

    public bool CreateDirectory(string path)
    {
        var (storage, relativePath) = resolve(path);
        return storage?.CreateDirectory(relativePath) ?? false;
    }

    public void Delete(string path)
    {
        var (storage, relativePath) = resolve(path);
        storage?.Delete(relativePath);
    }

    public void DeleteDirectory(string path)
    {
        var (storage, relativePath) = resolve(path);
        storage?.DeleteDirectory(relativePath);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        var (storage, relativePath) = resolve(path);
        return storage?.EnumerateDirectories(relativePath, pattern) ?? Array.Empty<string>();
    }

    public IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
        var (storage, relativePath) = resolve(path);
        return storage?.EnumerateFiles(relativePath, pattern, searchOptions) ?? Array.Empty<string>();
    }

    public bool Exists(string path)
    {
        var (storage, relativePath) = resolve(path);
        return storage?.Exists(relativePath) ?? false;
    }

    public bool ExistsDirectory(string path)
    {
        var (storage, relativePath) = resolve(path);
        return storage?.ExistsDirectory(relativePath) ?? false;
    }

    public Stream? Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        var (storage, relativePath) = resolve(path);
        return storage?.Open(relativePath, mode, access);
    }

    protected override void Destroy()
    {
        foreach (var storage in storages.Values)
        {
            if (storage is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private ResolveResult resolve(string path)
    {
        path = resolvePath(path);

        for (int i = path.Length; i >= 0; i--)
        {
            // if (path[i - 1] != Path.AltDirectorySeparatorChar)
            //     continue;

            if (!storages.TryGetValue(path[..i], out var storage))
                continue;

            string relativePath = i == path.Length ? string.Empty : path[i..];

            return new ResolveResult(storage, relativePath);
        }

        return default;
    }

    private static readonly string relative_to_parent = "..";
    private static readonly string relative_to_current = ".";

    private static string resolvePath(string path)
    {
        path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        string?[] components = path.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        for (int i = components.Length - 1; i >= 0; i--)
        {
            string? component = components[i];

            if (component == relative_to_parent)
            {
                components[i] = null;

                if (i > 0)
                    components[i - 1] = null;
            }

            if (component == relative_to_current)
            {
                components[i] = null;
            }
        }

        return Path.AltDirectorySeparatorChar + string.Join(Path.AltDirectorySeparatorChar, components.Where(c => !string.IsNullOrEmpty(c)));
    }

    private record struct ResolveResult(IStorage IStorage, string Path);
}
