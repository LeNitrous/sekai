// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridResourceLayout : VeldridGraphicsResource<Vd.ResourceLayout>, IResourceLayout
{
    public VeldridResourceLayout(Vd.ResourceLayout resource)
        : base(resource)
    {
    }
}
