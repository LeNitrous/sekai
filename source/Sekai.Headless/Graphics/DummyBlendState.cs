// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;

namespace Sekai.Headless.Graphics;

internal sealed class DummyBlendState : BlendState
{
    public override bool Enabled { get; }
    public override BlendType SourceColor { get; }
    public override BlendType DestinationColor { get; }
    public override BlendOperation ColorOperation { get; }
    public override BlendType SourceAlpha { get; }
    public override BlendType DestinationAlpha { get; }
    public override BlendOperation AlphaOperation { get; }
    public override ColorWriteMask WriteMask { get; }

    public override void Dispose()
    {
    }
}
