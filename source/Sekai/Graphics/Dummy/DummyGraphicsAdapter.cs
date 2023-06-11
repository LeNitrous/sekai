// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Graphics.Dummy;

internal sealed class DummyGraphicsAdapter : GraphicsAdapter
{
    public override string Name { get; } = "Dummy";

    public override string Vendor { get; } = "Dummy";

    public override Version Version { get; } = new Version();

    public override IEnumerable<GraphicsDeviceFeatures> GetDeviceFeatures()
    {
        return Enumerable.Empty<GraphicsDeviceFeatures>();
    }

    public override bool IsFeatureSupported(GraphicsDeviceFeatures feature)
    {
        return false;
    }
}
