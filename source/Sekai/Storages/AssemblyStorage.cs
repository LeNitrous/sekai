// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sekai.Storages;

/// <summary>
/// Storage backed by an <see cref="Assembly"/>.
/// </summary>
/// <remarks>
/// Assembly-backed storages are read-only. Their file entries may also be possibly change
/// compared to their local counterparts due to how assemblies name their manifests internally.
/// </remarks>
public class AssemblyStorage : Storage
{
    public override bool CanWrite => false;

    private readonly string prefix;
    private readonly Assembly assembly;
    private readonly IEnumerable<Uri> entries;

    public AssemblyStorage(Uri uri)
        : this(Assembly.LoadFile(uri.AbsolutePath))
    {
    }

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

                return GetFullPath(new Uri(new string(chars[prefix.Length..]), UriKind.Relative));
            });

        this.assembly = assembly;
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
        return entries;
    }

    protected override bool BaseExists(Uri uri)
    {
        return entries.Contains(uri, UriEqualityComparer.Default);
    }

    protected override bool BaseExistsDirectory(Uri uri)
    {
        return false;
    }

    protected override Stream BaseOpen(Uri uri, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (!Exists(uri))
            throw new FileNotFoundException(null, Path.GetFileName(uri.AbsolutePath));

        string name = prefix + uri.AbsolutePath.Replace(Path.AltDirectorySeparatorChar, separator);

        return assembly.GetManifestResourceStream(name)!;
    }

    private static readonly char separator = '.';
}
