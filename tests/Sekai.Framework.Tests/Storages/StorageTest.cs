// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using NUnit.Framework;
using Sekai.Framework.Storages;

namespace Sekai.Framework.Tests.Storages;

public abstract class StorageTest<T>
    where T : Storage
{
    private const string test_file = @"./file.txt";

    private const string test_path = @"./path/";

    private const string test_file_in_path = @"./path/file.csv";

    private const string test_path_in_path = @"./path/nest/";

    protected T Storage = null!;

    protected StorageTest()
    {
    }

    [SetUp]
    public void SetUp()
    {
        Storage = CreateStorage();

        if ((Access & FileAccess.Write) != 0)
        {
            if (HasDirectories)
            {
                Storage.CreateDirectory(test_path);
                Storage.CreateDirectory(test_path_in_path);
            }

            if (HasFiles)
            {
                Storage.Open(test_file, FileMode.CreateNew, FileAccess.Write).Dispose();
                Storage.Open(test_file_in_path, FileMode.CreateNew, FileAccess.Write).Dispose();
            }
        }
    }

    [TearDown]
    public void TearDown()
    {
        Storage.Dispose();
        Storage = null!;
    }

    [TestCase(FileMode.Append, FileAccess.Write)]
    [TestCase(FileMode.Create, FileAccess.Write)]
    [TestCase(FileMode.Create, FileAccess.ReadWrite)]
    [TestCase(FileMode.CreateNew, FileAccess.Write)]
    [TestCase(FileMode.CreateNew, FileAccess.ReadWrite)]
    [TestCase(FileMode.Open, FileAccess.Read)]
    [TestCase(FileMode.Open, FileAccess.Write)]
    [TestCase(FileMode.Open, FileAccess.ReadWrite)]
    [TestCase(FileMode.OpenOrCreate, FileAccess.Read)]
    [TestCase(FileMode.OpenOrCreate, FileAccess.Write)]
    [TestCase(FileMode.OpenOrCreate, FileAccess.ReadWrite)]
    [TestCase(FileMode.Truncate, FileAccess.Write)]
    [TestCase(FileMode.Truncate, FileAccess.ReadWrite)]
    public void Open_ReturnsStream(FileMode mode, FileAccess access)
    {
        if ((Access & access) != 0)
        {
            Stream? stream = null;
            Assert.That(() => stream = Storage.Open(mode is FileMode.Create or FileMode.CreateNew ? Guid.NewGuid().ToString() : test_file, mode, access), Is.InstanceOf<Stream>());
            stream?.Dispose();
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void Exists_ReturnsTrue()
    {
        Assert.That(Storage.Exists(test_file), HasFiles ? Is.True : Is.False);
    }

    [Test]
    public void Delete_ReturnsTrue()
    {
        Assert.That(Storage.Delete(test_file), HasFiles ? Is.True : Is.False);
    }

    [TestCase(SearchOption.TopDirectoryOnly)]
    [TestCase(SearchOption.AllDirectories)]
    public void EnumerateFiles_ReturnsIEnumerableOfString(SearchOption option)
    {
        if (HasFiles)
        {
            switch (option)
            {
                case SearchOption.TopDirectoryOnly:
                    Assert.That(Storage.EnumerateFiles(string.Empty, options: option), Has.Member(test_file.TrimStart('.')));
                    break;

                case SearchOption.AllDirectories:
                    Assert.That(Storage.EnumerateFiles(string.Empty, options: option), Has.Member(test_file_in_path.TrimStart('.')));
                    break;

                default:
                    Assert.Fail();
                    break;
            }
        }
        else
        {
            Assert.Pass();
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void OpenDirectory_ReturnsStorage(bool createIfNotExist)
    {
        if (HasDirectories)
        {
            Assert.That(() => Storage.OpenDirectory("./test/", createIfNotExist), createIfNotExist ? Is.InstanceOf<Storage>() : Throws.InstanceOf<DirectoryNotFoundException>());
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void ExistsDirectory_ReturnsTrue()
    {
        Assert.That(Storage.ExistsDirectory(test_path), HasDirectories ? Is.True : Is.False);
    }

    [Test]
    public void DeleteDirectory_ReturnsTrue()
    {
        Assert.That(Storage.DeleteDirectory(test_path), HasDirectories ? Is.True : Is.False);
    }

    [Test]
    public void CreateDirectory_ReturnsTrue()
    {
        Assert.That(Storage.CreateDirectory("./test/"), HasDirectories ? Is.True : Is.False);
    }

    [TestCase(SearchOption.TopDirectoryOnly)]
    [TestCase(SearchOption.AllDirectories)]
    public void EnumerateDirectories_ReturnsIEnumerableOfString(SearchOption option)
    {
        if (HasDirectories)
        {
            switch (option)
            {
                case SearchOption.TopDirectoryOnly:
                    Assert.That(Storage.EnumerateDirectories(string.Empty, options: option), Has.Member(test_path.TrimStart('.')));
                    break;

                case SearchOption.AllDirectories:
                    Assert.That(Storage.EnumerateDirectories(string.Empty, options: option), Has.Member(test_path_in_path.TrimStart('.')));
                    break;

                default:
                    Assert.Fail();
                    break;
            }
        }
        else
        {
            Assert.That(Storage.EnumerateDirectories(string.Empty, options: option), Is.Empty);
        }
    }

    /// <summary>
    /// Creates a storage for tests.
    /// </summary>
    protected abstract T CreateStorage();

    /// <summary>
    /// The nature of access of the given storage.
    /// </summary>
    protected virtual FileAccess Access => FileAccess.ReadWrite;

    /// <summary>
    /// Whether directories supported by the storage.
    /// </summary>
    protected virtual bool HasDirectories => true;

    /// <summary>
    /// Whether files supported by the storage.
    /// </summary>
    protected virtual bool HasFiles => true;
}
