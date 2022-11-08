// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Experimental.Mathematics.Tests;
internal class Vector2Test
{
    private static readonly Random random = new();

    [Test]
    public void Vector2ReadonlyFieldsTest()
    {
        Assert.That(Vector2.Zero, Is.EqualTo(new Vector2(0f, 0f)));
        Assert.That(Vector2.One, Is.EqualTo(new Vector2(1f, 1f)));
        Assert.That(Vector2.UnitX, Is.EqualTo(new Vector2(1f, 0f)));
        Assert.That(Vector2.UnitY, Is.EqualTo(new Vector2(0f, 1f)));
    }

    [Test]
    public void Vector2ConstructorTest()
    {
        var expectedValue = new Vector2(0f, 0f);
        Assert.That(Vector2.Zero, Is.EqualTo(expectedValue));
    }

    [Test]
    public void AdditionTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Add(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));
        var actual = left + right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void AdditionByRefTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Add(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));

        Vector2.Add(ref left, ref right, out var result);

        Assert.That(left, Is.EqualTo(leftExpected));
        Assert.That(right, Is.EqualTo(rightExpected));
        Assert.That(result, Is.EqualTo(resultExpected));
    }

    [Test]
    public void SubtractionTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Subtract(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));
        var actual = left - right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void SubtractionByRefTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Subtract(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));

        Vector2.Subtract(ref left, ref right, out var result);

        Assert.That(left, Is.EqualTo(leftExpected));
        Assert.That(right, Is.EqualTo(rightExpected));
        Assert.That(result, Is.EqualTo(resultExpected));
    }

    [Test]
    public void MultiplicationTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Multiply(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));
        var actual = left * right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void MultiplicationByRefTest()
    {
        // as with any arithmetic-by-ref, we have to make sure we have a copy to compare to
        var vector = new Vector2(random.NextSingle(), random.NextSingle());
        var expectedVector = vector;
        float scale = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Multiply(
            TestUtils.ConvertToSystemVec2(vector), scale));

        Vector2.Multiply(ref vector, scale, out var result);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(vector, Is.EqualTo(expectedVector));
    }

    [Test]
    public void DivisionTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Divide(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right)));
        var actual = left / right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DivisionByRefTest()
    {
        // as with any arithmetic-by-ref, we have to make sure we have a copy to compare to
        var vector = new Vector2(random.NextSingle(), random.NextSingle());
        var expectedVector = vector;
        float scale = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec2(System.Numerics.Vector2.Divide(
            TestUtils.ConvertToSystemVec2(vector), scale));

        Vector2.Divide(ref vector, scale, out var result);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(vector, Is.EqualTo(expectedVector));
    }

    [Test]
    public void DotTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());
        float expected = System.Numerics.Vector2.Dot(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right));
        float actual = Vector2.Dot(left, right);

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DotByRefTest()
    {
        var left = new Vector2(random.NextSingle(), random.NextSingle());
        var right = new Vector2(random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        float expected = System.Numerics.Vector2.Dot(
            TestUtils.ConvertToSystemVec2(left), TestUtils.ConvertToSystemVec2(right));

        Vector2.Dot(ref left, ref right, out float result);

        Assert.That(left, Is.EqualTo(leftExpected));
        Assert.That(right, Is.EqualTo(rightExpected));
        Assert.That(result, Is.EqualTo(expected));
    }
}
