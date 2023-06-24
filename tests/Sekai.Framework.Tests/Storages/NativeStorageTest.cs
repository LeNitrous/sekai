// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using Sekai.Framework.Storages;

namespace Sekai.Framework.Tests.Storages;

public class NativeStorageTest : StorageTest<NativeStorage>
{
    public NativeStorageTest()
    {
    }

    protected override NativeStorage CreateStorage() => new TemporaryNativeStorage();

    private class TemporaryNativeStorage : NativeStorage
    {
        private bool isDisposed;

        public TemporaryNativeStorage()
            : base($"{Path.GetTempPath()}/{nameof(TemporaryNativeStorage)}-{Guid.NewGuid()}/")
        {
            Directory.Create();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (isDisposed)
            {
                return;
            }

            Directory.Delete(true);

            isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
