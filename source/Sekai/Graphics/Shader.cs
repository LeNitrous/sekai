// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

public abstract class Shader : IDisposable
{
    /// <summary>
    /// The stages this shader performs.
    /// </summary>
    public abstract ShaderStage Stages { get; }

    public abstract void Dispose();
}
