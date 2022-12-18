// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Rendering;

public class Renderer3D : Renderer<Drawable3D, Camera3D>
{
    protected override IComparer<Drawable3D> CreateComparer() => Comparer<Drawable3D>.Create((x, y) => x.Transform.CompareTo(y.Transform));
}
