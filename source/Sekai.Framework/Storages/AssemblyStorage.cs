// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Sekai.Framework.Storages;

/// <summary>
/// A kind of <see cref="Storage"/> that is backed by an <see cref="Assembly"/>.
/// </summary>
/// <remarks>
/// This type of storage is read-only and only files can be accessed.
/// </remarks>
public sealed class AssemblyStorage : Storage
{
    private readonly string prefix;
    private readonly string[] entries;
    private readonly Assembly assembly;
    private const char separator = '.';

#pragma warning disable IL2026 // We will not reference any types and members. Only embedded resources.

    public AssemblyStorage(string path)
        : this(Assembly.LoadFile(path))
    {
    }

#pragma warning restore IL2026

    public AssemblyStorage(AssemblyName name)
        : this(Assembly.Load(name))
    {
    }

    public AssemblyStorage(Assembly assembly)
    {
        prefix = assembly.GetName().Name ?? string.Empty;

        entries = assembly
            .GetManifestResourceNames()
            .Select(name =>
            {
                Span<char> chars = stackalloc char[name.Length];

                for (int i = name.Length - 1; i >= 0; i--)
                    chars[i] = name[i];

                for (int i = MemoryExtensions.LastIndexOf(chars, separator) - 1; i >= 0; i--)
                {
                    if (chars[i] == separator)
                        chars[i] = Path.AltDirectorySeparatorChar;
                }

                return new string(chars[prefix.Length..]);
            })
            .ToArray();

        this.assembly = assembly;
    }

    public override Stream Open([StringSyntax("Uri")] string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!Exists(path))
        {
            throw new FileNotFoundException($"File {path} is not found.", nameof(path));
        }

        if (access != FileAccess.Read)
        {
            throw new ArgumentException("Cannot write to a read-only storage.", nameof(access));
        }

        switch (mode)
        {
            case FileMode.CreateNew:
            case FileMode.Create:
            case FileMode.OpenOrCreate:
            case FileMode.Truncate:
            case FileMode.Append:
                throw new ArgumentException("Cannot modify contents of a read-only stream.", nameof(mode));
        }

        string name = prefix + path.Replace(Path.AltDirectorySeparatorChar, separator);

        return assembly.GetManifestResourceStream(name)!;
    }

    public override bool Exists([StringSyntax("Uri")] string path)
    {
        return entries.Contains(path);
    }

    public override bool Delete([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool ExistsDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool DeleteDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override bool CreateDirectory([StringSyntax("Uri")] string path)
    {
        return false;
    }

    public override IEnumerable<string> EnumerateFiles([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        return matcher.Match(path).Files.Select(f => f.Path);
    }

    public override IEnumerable<string> EnumerateDirectories([StringSyntax("Uri")] string path, string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
    {
        return Enumerable.Empty<string>();
    }

    public override void Dispose()
    {
    }
}
