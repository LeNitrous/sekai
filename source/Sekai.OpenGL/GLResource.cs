// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal abstract class GLResource : FrameworkObject
{
    protected GLGraphicsSystem Context { get; }
    protected GL GL => Context.GL;

    protected GLResource(GLGraphicsSystem context)
    {
        Context = context;
    }
}
