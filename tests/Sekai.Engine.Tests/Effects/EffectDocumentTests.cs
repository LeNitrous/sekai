// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Tests.Effects;

public class EffectDocumentTests
{
    [Test]
    public void TestParseRoot()
    {
        var document = EffectDocument.Load(@"effect ""Test"" { pass ""Default"" {} }");
        Assert.Multiple(() =>
        {
            Assert.That(document.Name, Is.EqualTo("Test"));
            Assert.That(document.Passes, Is.Not.Empty);
            Assert.That(document.Passes[0].Name, Is.EqualTo("Default"));
        });
    }

    [Test]
    public void TestParseStruct()
    {
        var document = EffectDocument.Load(@"
effect ""Test""
{
    pass ""Default""
    {
        struct Test
        {
            float Scalar;
            vec4 VectorWithArray[1];
        }
    }
}
");

        Assert.Multiple(() =>
        {
            Assert.That(document.Passes[0].Structs, Is.Not.Empty);
            Assert.That(document.Passes[0].Structs[0].Name, Is.EqualTo("Test"));
            Assert.That(document.Passes[0].Structs[0].Members, Is.Not.Empty);
            Assert.That(document.Passes[0].Structs[0].Members[0].Type, Is.EqualTo("float"));
            Assert.That(document.Passes[0].Structs[0].Members[0].Name, Is.EqualTo("Scalar"));
            Assert.That(document.Passes[0].Structs[0].Members[1].Type, Is.EqualTo("vec4"));
            Assert.That(document.Passes[0].Structs[0].Members[1].Name, Is.EqualTo("VectorWithArray"));
            Assert.That(document.Passes[0].Structs[0].Members[1].Size, Is.EqualTo("1"));
        });
    }

    [Test]
    public void TestParseMember()
    {
        var document = EffectDocument.Load(@"
effect ""Test""
{
    pass ""Default""
    {
        uniform mat4 Uniform;
        buffer vec3 Buffer[10];
    }
}
");

        Assert.Multiple(() =>
        {
            Assert.That(document.Passes[0].Members, Is.Not.Empty);
            Assert.That(document.Passes[0].Members[0].Type, Is.EqualTo("mat4"));
            Assert.That(document.Passes[0].Members[0].Name, Is.EqualTo("Uniform"));
            Assert.That(document.Passes[0].Members[0].Qualifier, Is.EqualTo("uniform"));
            Assert.That(document.Passes[0].Members[1].Type, Is.EqualTo("vec3"));
            Assert.That(document.Passes[0].Members[1].Name, Is.EqualTo("Buffer"));
            Assert.That(document.Passes[0].Members[1].Size, Is.EqualTo("10"));
            Assert.That(document.Passes[0].Members[1].Qualifier, Is.EqualTo("buffer"));
        });
    }

    [Test]
    public void TestParseMethod()
    {
        var document = EffectDocument.Load(@"
effect ""Test""
{
    pass ""Default""
    {
        int main(int arg1)
        {
            return 0;
        }
    }
}
");

        Assert.Multiple(() =>
        {
            Assert.That(document.Passes[0].Methods, Is.Not.Empty);
            Assert.That(document.Passes[0].Methods[0].Type, Is.EqualTo("int"));
            Assert.That(document.Passes[0].Methods[0].Name, Is.EqualTo("main"));
            Assert.That(document.Passes[0].Methods[0].Body, Is.Not.Empty);
            Assert.That(document.Passes[0].Methods[0].Parameters, Is.Not.Empty);
            Assert.That(document.Passes[0].Methods[0].Parameters[0].Type, Is.EqualTo("int"));
            Assert.That(document.Passes[0].Methods[0].Parameters[0].Name, Is.EqualTo("arg1"));
        });
    }

    [Test]
    public void TestParseDirective()
    {
        var document = EffectDocument.Load(@"
effect ""Test""
{
    pass ""Default""
    {
        #include ""scene.sksl""
    }
}
");

        Assert.Multiple(() =>
        {
            Assert.That(document.Passes[0].Directives, Is.Not.Empty);
            Assert.That(document.Passes[0].Directives[0].Key, Is.EqualTo("include"));
            Assert.That(document.Passes[0].Directives[0].Value, Is.EqualTo("\"scene.sksl\""));
        });
    }
}
