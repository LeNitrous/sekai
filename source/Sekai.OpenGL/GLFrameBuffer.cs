// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Buffers;
using Sekai.Graphics.Textures;

namespace Sekai.OpenGL;

internal class GLFrameBuffer : FrameworkObject, INativeFrameBuffer
{
    public void AddAttachment(INativeTexture texture, int level, int layer)
    {
    }
}
