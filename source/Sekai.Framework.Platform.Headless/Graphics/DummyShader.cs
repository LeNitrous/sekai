// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;

namespace Sekai.Framework.Platform.Headless.Graphics;

internal sealed class DummyShader : Shader
{
    public override ShaderStage Stages { get; }

    public DummyShader(ShaderCode[] attachments)
    {
        for (int i = 0; i < attachments.Length; i++)
        {
            Stages |= attachments[i].Stage;
        }
    }

    public override void Dispose()
    {
    }
}
