// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;

namespace Sekai.Engine.Effects;

public abstract class EffectResolver : FrameworkObject
{
    /// <summary>
    /// Resolves an effect at a given path.
    /// </summary>
    public abstract string Resolve(string path);
}
