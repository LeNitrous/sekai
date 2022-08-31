// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Graphics;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Extensions;

public static class CommandQueueExtensions
{
    public static void SetVertexBuffer(this ICommandQueue queue, VertexBuffer buffer)
    {
        buffer.Bind(queue);
    }

    public static void SetIndexBuffer(this ICommandQueue queue, IndexBuffer buffer)
    {
        buffer.Bind(queue);
    }
}
