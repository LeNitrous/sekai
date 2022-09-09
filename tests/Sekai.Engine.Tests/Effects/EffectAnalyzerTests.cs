using NUnit.Framework;
using Sekai.Engine.Effects.Compiler;
using Sekai.Engine.Effects.Documents;

namespace Sekai.Engine.Tests.Effects;

public class EffectAnalyzerTests
{
    [Test]
    public void TestAnalyzeParameterScalar()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform float Float;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Float"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("float"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(4));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform));
        });
    }

    [Test]
    public void TestAnalyzeParameterBuiltin()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform vec3 Vector;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Vector"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("vec3"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(16));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform));
        });
    }

    [Test]
    public void TestAnalyzeParameterImage()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform image2D Image;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Image"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("image2D"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(0));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform | EffectParameterFlags.Image));
        });
    }

    [Test]
    public void TestAnalyzeParameterSampler()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform sampler Sampler;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Sampler"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("sampler"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(0));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform | EffectParameterFlags.Sampler));
        });
    }

    [Test]
    public void TestAnalyzeParameterTexture()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform texture2D Texture;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Texture"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("texture2D"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(0));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform | EffectParameterFlags.Texture));
        });
    }

    [Test]
    public void TestAnalyzeParameterScalarArray()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform float Floats[3];
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Floats"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("float"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(48));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform));
        });
    }

    [Test]
    public void TestAnalyzeParameterTextureCubemap()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        uniform textureCube Cubemap;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("Cubemap"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("textureCube"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(0));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Uniform | EffectParameterFlags.Texture | EffectParameterFlags.Cubemap));
        });
    }

    [Test]
    public void TestAnalyzeStruct()
    {
        string source = @"
effect ""Test""
{
    pass ""Default""
    {
        struct TestStruct
        {
            float Float1;
            float Float2;
            float Float3;
        }

        buffer TestStruct TestBuffer;
    }
}
";

        var document = EffectDocument.Load(source);
        var analyzer = EffectAnalyzer.Analyze(document.Passes[0]);

        Assert.Multiple(() =>
        {
            Assert.That(analyzer.Parameters, Is.Not.Empty);
            Assert.That(analyzer.Parameters[0].Name, Is.EqualTo("TestBuffer"));
            Assert.That(analyzer.Parameters[0].Type, Is.EqualTo("TestStruct"));
            Assert.That(analyzer.Parameters[0].Size, Is.EqualTo(16));
            Assert.That(analyzer.Parameters[0].Flags, Is.EqualTo(EffectParameterFlags.Buffer));
        });
    }
}
