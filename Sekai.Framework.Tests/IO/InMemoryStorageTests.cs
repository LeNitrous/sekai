// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;
using NUnit.Framework;
using Sekai.Framework.IO.Storage;

namespace Sekai.Framework.Tests.IO;

public class InMemoryStorageTests
{
    [Test]
    public void TestStorageReadWrite()
    {
        var storage = new InMemoryStorage();

        using (var writeStream = storage.Open("test.txt", FileMode.Create, FileAccess.Write))
        {
            Assert.That(writeStream, Is.Not.Null);

            if (writeStream != null)
            {
                using var writer = new StreamWriter(writeStream);
                writer.Write("Hello World");
            }
        }

        using var readStream = storage.Open("test.txt", FileMode.Open, FileAccess.Read);
        Assert.That(readStream, Is.Not.Null);

        if (readStream != null)
        {
            using var reader = new StreamReader(readStream);
            Assert.That(reader.ReadToEnd(), Is.EqualTo("Hello World"));
        }
    }
}
