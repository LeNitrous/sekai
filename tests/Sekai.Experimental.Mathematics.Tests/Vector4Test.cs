// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Experimental.Mathematics.Tests;
internal class Vector4Test
{
    private static readonly Random random = new();

    [SetUp]
    // Vector4 Readonly fields correctness
    [Test]
    public void Vector4ReadonlyFieldsTest()
    {
        Assert.That(Vector4.Zero, Is.EqualTo(new Vector4(0f, 0f, 0f, 0f)));
        Assert.That(Vector4.One, Is.EqualTo(new Vector4(1f, 1f, 1f, 1f)));
        Assert.That(Vector4.UnitX, Is.EqualTo(new Vector4(1f, 0f, 0f, 0f)));
        Assert.That(Vector4.UnitY, Is.EqualTo(new Vector4(0f, 1f, 0f, 0f)));
        Assert.That(Vector4.UnitZ, Is.EqualTo(new Vector4(0f, 0f, 1f, 0f)));
        Assert.That(Vector4.UnitW, Is.EqualTo(new Vector4(0f, 0f, 0f, 1f)));
    }

    // Vector4 ctor correctness test
    [Test]
    public void Vector4ConstructorTest()
    {
        var expectedValue = new Vector4(0f, 0f, 0f, 0f);
        Assert.That(Vector4.Zero, Is.EqualTo(expectedValue));
    }

    //Vector4 arithmetic correctness test
    [Test]
    public void AdditionTest()
    {
        // The way we're doing this is we're comparing if Vector4 from
        // System.Numerics is the same as our implementation
        // That way, we have a baseline of sanity.
        var left = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec4(System.Numerics.Vector4.Add(
            TestUtils.ConvertToSystemVec4(left), TestUtils.ConvertToSystemVec4(right)));
        var actual = left + right;

        Assert.That(expected, Is.EqualTo(actual));

    }

    [Test]
    public void AdditionByRefTest()
    {
        var left = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemVec4(System.Numerics.Vector4.Add(
            TestUtils.ConvertToSystemVec4(left), TestUtils.ConvertToSystemVec4(right)));


        Vector4.Add(ref left, ref right, out var result);

        // We need to assert if left and right is still the same value after
        // the add-by-ref since pass by ref is, well, just passing a ptr ref to the var.
        Assert.That(resultExpected, Is.EqualTo(result));
        Assert.That(leftExpected, Is.EqualTo(left));
        Assert.That(rightExpected, Is.EqualTo(right));
    }

    [Test]
    public void DivisionTest()
    {
        var vector = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        float scale = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec4(System.Numerics.Vector4.Divide(
            TestUtils.ConvertToSystemVec4(vector), scale));

        // we could use Vector4.Divide(), but we already implemented the / operator.
        var actual = vector / scale;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DivisionByRefTest()
    {
        // as with any arithmetic-by-ref, we have to make sure we have a copy to compare to
        var vector = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expectedVector = vector;
        float scale = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec4(System.Numerics.Vector4.Divide(
            TestUtils.ConvertToSystemVec4(vector), scale));

        Vector4.Divide(ref vector, scale, out var result);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(vector, Is.EqualTo(expectedVector));
    }

    [Test]
    public void DotTest()
    {
        var left = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());

        float expected = System.Numerics.Vector4.Dot(TestUtils.ConvertToSystemVec4(left), TestUtils.ConvertToSystemVec4(right));
        float actual = Vector4.Dot(left, right);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DotByRefTest()
    {
        var left = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());

        var expectedLeft = left;
        var expectedRight = right;

        float expected = System.Numerics.Vector4.Dot(TestUtils.ConvertToSystemVec4(left), TestUtils.ConvertToSystemVec4(right));

        Vector4.Dot(ref left, ref right, out float actual);

        Assert.That(actual, Is.EqualTo(expected));
        Assert.That(left, Is.EqualTo(expectedLeft));
        Assert.That(right, Is.EqualTo(expectedRight));
    }
}
