// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Globbing;

namespace Sekai.Framework.Storage;

public class AssemblyBackedStorage : IStorage
{
    private static readonly char separator = '.';
    private readonly string prefix;
    private readonly Assembly assembly;
    private readonly IEnumerable<string> entries;

    public AssemblyBackedStorage(Assembly assembly)
    {
        this.assembly = assembly;

        prefix = assembly.GetName().Name ?? string.Empty;
        entries = assembly.GetManifestResourceNames().Select(name =>
        {
            char[] chars = name[(name.StartsWith(prefix, StringComparison.Ordinal) ? prefix.Length + 1 : 0)..].ToCharArray();

            for (int i = Array.LastIndexOf(chars, separator) - 1; i >= 0; i--)
            {
                if (chars[i] == separator)
                    chars[i] = Path.AltDirectorySeparatorChar;
            }

            return new string(chars);
        });
    }

    public AssemblyBackedStorage(AssemblyName name)
        : this(Assembly.Load(name))
    {
    }

    public bool CreateDirectory(string path)
    {
        throw new NotSupportedException(@"Cannot create directories in an assembly manifest.");
    }

    public void Delete(string path)
    {
        throw new UnauthorizedAccessException(@"Assembly manifests are read-only.");
    }

    public void DeleteDirectory(string path)
    {
        throw new NotSupportedException(@"Cannot delete directories in an assembly manifest.");
    }

    public IEnumerable<string> EnumerateDirectories(string path, string pattern = "*")
    {
        throw new NotSupportedException(@"Cannot enumerate directories in an assembly manifest.");
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
        throw new NotSupportedException(@"Cannot enumerate directories in an assembly manifest.");
    }

    public Stream? Open(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!Exists(path))
            throw new FileNotFoundException(null, path);

        if (mode != FileMode.Open || access != FileAccess.Read)
            throw new UnauthorizedAccessException(@"Archives are read-only");

        string manifest = prefix + separator + path.Replace(Path.AltDirectorySeparatorChar, separator);
        return assembly.GetManifestResourceStream(manifest);
    }
}
