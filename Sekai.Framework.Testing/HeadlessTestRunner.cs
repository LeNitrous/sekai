// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Platform;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Testing;

internal class HeadlessTestRunner : HeadlessHost
{
    public new void CheckForExceptions()
    {
        base.CheckForExceptions();
    }

    protected sealed override GameThreadManager CreateThreadManager()
    {
        return new HeadlessThreadManager();
    }
}
