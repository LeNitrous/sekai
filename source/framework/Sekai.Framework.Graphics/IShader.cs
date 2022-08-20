// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface IShader : IGraphicsResource
{
    /// <summary>
    /// The shader stage this shader is used in.
    /// </summary>
    ShaderStage Stage { get; }

    /// <summary>
    /// The name of the entry point function.
    /// </summary>
    string EntryPoint { get; }
}
