// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;
using NUnit.Framework;
using Sekai.Storage;

namespace Sekai.Tests.Storage;

public class StorageContextTest
{
    [Test]
    public void TestStorageMounting()
    {
        var storage = new StorageContext();
        storage.Mount("a", createTestStorage("a.txt", "hello"));
        Assert.That(storage.Exists("a/a.txt"), Is.True);
    }

    [Test]
    public void TestStorageMountingRoot()
    {
        var storage = new StorageContext(createTestStorage("a.txt", "hello"));
        Assert.That(storage.Exists("a.txt"), Is.True);
    }

    [Test]
    public void TestStorageMountingNested()
    {
        var storage = new StorageContext();
        storage.Mount("a/b/c", createTestStorage("a.txt", "hello"));
        Assert.That(storage.Exists("a/b/c/a.txt"), Is.True);
    }

    private IStorage createTestStorage(string filename, string content)
    {
        var memory = new MemoryStorage();

        using var stream = memory.Open(filename);
        using var writer = new StreamWriter(stream);
        writer.Write(content);

        return memory;
    }
}
