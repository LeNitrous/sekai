// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridResourceSet : VeldridGraphicsResource<Vd.ResourceSet>, IResourceSet
{
    public VeldridResourceSet(Vd.ResourceSet resource)
        : base(resource)
    {
    }
}
