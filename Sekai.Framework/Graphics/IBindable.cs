// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface IBindable
{
    /// <summary>
    /// Binds this resource to the given command list for rendering.
    /// </summary>
    void Bind(CommandList commands);
}
