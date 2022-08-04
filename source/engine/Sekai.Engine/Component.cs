// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Annotations;

namespace Sekai.Engine;

public abstract class Component : ActivatableObject
{
    [Resolved]
    public Entity Entity = null!;

    private protected override bool CanCache => false;
}
