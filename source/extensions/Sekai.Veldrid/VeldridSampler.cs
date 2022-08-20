// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridSampler : VeldridGraphicsResource<Vd.Sampler>, ISampler
{
    public VeldridSampler(Vd.Sampler resource)
        : base(resource)
    {
    }
}
