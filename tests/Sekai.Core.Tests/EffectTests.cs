// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Graphics.Effects;

namespace Sekai.Core.Tests;

public class EffectTests : GraphicsDeviceTest
{
    [Test]
    public void Create_Effect_Succeeds()
    {
        const string code = @"
attrib vec4 a_position;
attrib vec2 a_texCoord;

extern mat4 m_proj;
extern mat4 m_view;
extern mat4 m_model;
extern texture2D m_albedo;

void vert()
{
    SK_POSITION = m_proj * m_view * m_model * a_position;
}

void frag()
{
    SK_COLOR0 = texture(m_albedo, a_texCoord);
}
";

        Effect effect = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => effect = Effect.Create(Graphics, code), Throws.Nothing);
            Assert.That(effect.HasParameter<EffectValueParameter>("m_proj"), Is.True);
            Assert.That(effect.GetParameter<EffectValueParameter>("m_proj").Offset, Is.EqualTo(0));
            Assert.That(effect.GetParameter<EffectValueParameter>("m_proj").Size, Is.EqualTo(64));
            Assert.That(effect.HasParameter<EffectValueParameter>("m_view"), Is.True);
            Assert.That(effect.GetParameter<EffectValueParameter>("m_view").Offset, Is.EqualTo(64));
            Assert.That(effect.GetParameter<EffectValueParameter>("m_view").Size, Is.EqualTo(64));
            Assert.That(effect.HasParameter<EffectValueParameter>("m_model"), Is.True);
            Assert.That(effect.GetParameter<EffectValueParameter>("m_model").Offset, Is.EqualTo(128));
            Assert.That(effect.GetParameter<EffectValueParameter>("m_model").Size, Is.EqualTo(64));
            Assert.That(effect.HasParameter<EffectOpaqueParameter>("m_albedo"), Is.True);
        });
    }

    [Test]
    public void Create_Effect_Fails_Unsupported()
    {
        const string code = @"
attrib vec4 a_position;
attrib vec2 a_texCoord;

extern mat4 m_proj;
extern mat4 m_view;
extern mat4 m_model;
extern texture2D m_albedo;

void vert()
{
    SK_POSITION = m_proj * m_view * m_model * a_position;
}
";

        Assert.That(() => Effect.Create(Graphics, code), Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void Create_Effect_Fails_Syntax()
    {
        const string code = @"
attrib vec4 a_position;
attrib vec2 a_texCoord;

extern mat4 m_proj;
extern mat4 m_view;
extern mat4 m_model;
extern texture2D m_albedo;

void vert()
{
    // This line is missing a semi-colon at the end.
    SK_POSITION = m_proj * m_view * m_model * a_position
}

void frag()
{
    SK_COLOR0 = texture(m_albedo, a_texCoord);
}
";

        Assert.That(() => Effect.Create(Graphics, code), Throws.InstanceOf<EffectCompilationException>());
    }
}
