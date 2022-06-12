// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Silk.NET.Windowing;

namespace Sekai.Framework.Platform;

public class MobileHost : ViewHost
{
    protected override IView CreateView(ViewOptions opts)
    {
        return Window.GetView(opts);
    }
}
