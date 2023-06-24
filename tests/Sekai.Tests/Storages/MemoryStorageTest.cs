// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Storages;

namespace Sekai.Tests.Storages;

public class MemoryStorageTest : StorageTest<MemoryStorage>
{
    public MemoryStorageTest()
    {
    }

    protected override MemoryStorage CreateStorage() => new();
}
