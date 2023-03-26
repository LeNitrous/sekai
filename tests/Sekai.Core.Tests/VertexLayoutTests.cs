// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using NUnit.Framework;
using Sekai.Graphics;
using Veldrid;

namespace Sekai.Core.Tests;

public class VertexLayoutTests
{
    [Test]
    public void Create_From_Type()
    {
        VertexLayout layout = null!;

        Assert.Multiple(() =>
        {
            Assert.That(() => layout = VertexLayout.Create<TestLayout>(), Throws.Nothing);
            Assert.That(layout.Members, Has.Count.EqualTo(3));
            Assert.That(layout.Members[0].Name, Is.EqualTo("Position"));
            Assert.That(layout.Members[0].Format, Is.EqualTo(VertexElementFormat.Float2));
            Assert.That(layout.Members[0].Offset, Is.EqualTo(0));
            Assert.That(layout.Members[1].Name, Is.EqualTo("Color"));
            Assert.That(layout.Members[1].Format, Is.EqualTo(VertexElementFormat.Float4));
            Assert.That(layout.Members[1].Offset, Is.EqualTo(8));
            Assert.That(layout.Members[2].Name, Is.EqualTo("AnotherField"));
            Assert.That(layout.Members[2].Format, Is.EqualTo(VertexElementFormat.Float1));
            Assert.That(layout.Members[2].Offset, Is.EqualTo(24));
        });
    }

    [Test]
    public void Create_From_Builder()
    {
        VertexLayout layout = null!;
        var builder = new VertexLayoutBuilder();

        Assert.Multiple(() =>
        {
            Assert.That(() => builder.Add<Vector2>("Position"), Throws.Nothing);
            Assert.That(() => builder.Add<Vector4>("Color"), Throws.Nothing);
            Assert.That(() => builder.Add<float>("Test"), Throws.Nothing);
            Assert.That(() => builder.Add("Flag", 1, VertexMemberFormat.Int), Throws.Nothing);
            Assert.That(() => builder.Add<double>("Invalid"), Throws.InstanceOf<NotSupportedException>());
            Assert.That(() => layout = builder.Build(), Throws.Nothing);
            Assert.That(layout.Members, Has.Count.EqualTo(4));
            Assert.That(layout.Members[0].Name, Is.EqualTo("Position"));
            Assert.That(layout.Members[0].Format, Is.EqualTo(VertexElementFormat.Float2));
            Assert.That(layout.Members[0].Offset, Is.EqualTo(0));
            Assert.That(layout.Members[1].Name, Is.EqualTo("Color"));
            Assert.That(layout.Members[1].Format, Is.EqualTo(VertexElementFormat.Float4));
            Assert.That(layout.Members[1].Offset, Is.EqualTo(8));
            Assert.That(layout.Members[2].Name, Is.EqualTo("Test"));
            Assert.That(layout.Members[2].Format, Is.EqualTo(VertexElementFormat.Float1));
            Assert.That(layout.Members[2].Offset, Is.EqualTo(24));
            Assert.That(layout.Members[3].Name, Is.EqualTo("Flag"));
            Assert.That(layout.Members[3].Format, Is.EqualTo(VertexElementFormat.Int1));
            Assert.That(layout.Members[3].Offset, Is.EqualTo(28));
        });
    }

    private struct TestLayout : IVertex
    {
        [VertexMember(2, VertexMemberFormat.Float)]
        public Vector2 Position;

        [VertexMember]
        public Vector4 Color;

        public float Field;

        [VertexMember]
        public TestLayoutInner Inner;
    }

    private struct TestLayoutInner : IVertex
    {
        [VertexMember]
        public float AnotherField;
    }
}
