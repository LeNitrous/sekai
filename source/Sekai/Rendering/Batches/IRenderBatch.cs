// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public interface IRenderBatch
{
    bool HasStarted { get; }
    void Begin();
    void Flush();
    void End();
}

public interface IRenderBatch<T> : IRenderBatch
    where T : unmanaged, IVertex
{
    void Add(T vertex);
}
