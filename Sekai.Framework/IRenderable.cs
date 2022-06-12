// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework;

public interface IRenderable
{
    void Render(CommandList commands);
}
