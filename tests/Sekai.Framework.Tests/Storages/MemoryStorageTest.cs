// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Storages;

namespace Sekai.Framework.Tests.Storages;

public class MemoryStorageTest : StorageTest<MemoryStorage>
{
    public MemoryStorageTest()
    {
    }

    protected override MemoryStorage CreateStorage() => new();
}
