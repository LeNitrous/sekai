// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

/// <summary>
/// A host capable of running in the background.
/// </summary>
public class HeadlessHost : Host
{
    public HeadlessHost()
        : base(null)
    {
    }

    protected override IGraphicsContext CreateGraphicsContext(GraphicsAPI api) => new GraphicsContext(api);
    protected override FrameworkThreadManager CreateThreadManager() => new HeadlessThreadManager();

    protected class HeadlessThreadManager : FrameworkThreadManager
    {
        protected override MainThread CreateMainThread() => new();
    }
}
