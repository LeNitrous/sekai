// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public class HeadlessHost : Host
{
    protected override GameThread CreateMainThread()
    {
        return new GameThread("Main");
    }
}
