// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;
using NUnit.Framework;
using Sekai.Storage;

namespace Sekai.Tests.Storage;

public class MemoryStorageTest
{
    [Test]
    public void TestFileOpen()
    {
        var memory = createStorage();

        Assert.Multiple(() =>
        {
            Assert.That(() => memory.Open("a.txt", FileMode.Open), Throws.InstanceOf<FileNotFoundException>());
            Assert.That(() => memory.Open("test.txt", FileMode.CreateNew), Throws.InstanceOf<IOException>());
        });
    }

    [Test]
    public void TestFileReadWrite()
    {
        var memory = createStorage();

        Assert.That(memory.Exists("test.txt"), Is.True);

        using (var stream = memory.Open("test.txt", FileMode.Open))
        {
            using var reader = new StreamReader(stream);
            Assert.That(reader.ReadToEnd(), Is.EqualTo("Hello World"));
        }
    }

    [Test]
    public void TestFileAppend()
    {
        var memory = createStorage();

        using (var stream = memory.Open("test.txt", FileMode.Append))
        {
            using var writer = new StreamWriter(stream);
            writer.Write("Hello World");
        }

        using (var stream = memory.Open("test.txt", FileMode.Open))
        {
            using var reader = new StreamReader(stream);
            Assert.That(reader.ReadToEnd(), Is.EqualTo("Hello WorldHello World"));
        }
    }

    [Test]
    public void TestFileTruncate()
    {
        var memory = createStorage();

        using (var stream = memory.Open("test.txt", FileMode.Truncate))
        {
            using var writer = new StreamWriter(stream);
            writer.Write("Goodbye World");
        }

        using (var stream = memory.Open("test.txt", FileMode.Open))
        {
            using var reader = new StreamReader(stream);
            Assert.That(reader.ReadToEnd(), Is.EqualTo("Goodbye World"));
        }
    }

    private static MemoryStorage createStorage()
    {
        var memory = new MemoryStorage();

        using (var stream = memory.Open("test.txt", FileMode.CreateNew))
        {
            using var writer = new StreamWriter(stream);
            writer.Write("Hello World");
        }

        return memory;
    }
}
