// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Mathematics;

namespace Sekai.Tests.Mathematics;
internal class Vector3Test
{
    private static readonly Random random = new();

    // Vector3 Readonly fields correctness
    [Test]
    public void Vector3ReadonlyFieldsTest()
    {
        Assert.That(Vector3.Zero, Is.EqualTo(new Vector3(0f, 0f, 0f)));
        Assert.That(Vector3.One, Is.EqualTo(new Vector3(1f, 1f, 1f)));
        Assert.That(Vector3.UnitX, Is.EqualTo(new Vector3(1f, 0f, 0f)));
        Assert.That(Vector3.UnitY, Is.EqualTo(new Vector3(0f, 1f, 0f)));
        Assert.That(Vector3.UnitZ, Is.EqualTo(new Vector3(0f, 0f, 1f)));
    }

    // Vector3 ctor correctness test
    [Test]
    public void Vector3ConstructorTest()
    {
        var expectedValue = new Vector3(0f, 0f, 0f);
        Assert.That(Vector3.Zero, Is.EqualTo(expectedValue));
    }

    //Vector3 arithmetic correctness test
    [Test]
    public void AdditionTest()
    {
        // The way we're doing this is we're comparing if Vector3 from
        // System.Numerics is the same as our implementation
        // That way, we have a baseline of sanity.
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Add(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));
        var actual = left + right;

        Assert.That(expected, Is.EqualTo(actual));

    }

    [Test]
    public void AdditionByRefTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Add(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));

        Vector3.Add(ref left, ref right, out var result);

        Assert.That(leftExpected, Is.EqualTo(left));
        Assert.That(rightExpected, Is.EqualTo(right));
        Assert.That(resultExpected, Is.EqualTo(result));
    }

    [Test]
    public void SubtractionTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Subtract(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));
        var actual = left - right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void SubtractionByRefTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Subtract(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));

        Vector3.Subtract(ref left, ref right, out var result);

        Assert.That(leftExpected, Is.EqualTo(left));
        Assert.That(rightExpected, Is.EqualTo(right));
        Assert.That(resultExpected, Is.EqualTo(result));
    }

    [Test]
    public void MultiplicationTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Multiply(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));
        var actual = left * right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void MultiplicationByRefTest()
    {
        var vector = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expectedVector = vector;

        float scalar = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Multiply(
            TestUtils.ConvertToSystemVec3(vector), scalar));

        Vector3.Multiply(ref vector, scalar, out var actual);

        Assert.That(expectedVector, Is.EqualTo(vector));
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DivisionTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Divide(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right)));
        var actual = left / right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DivisionByRefTest()
    {
        var vector = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expectedVector = vector;

        float scalar = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemVec3(System.Numerics.Vector3.Divide(
            TestUtils.ConvertToSystemVec3(vector), scalar));

        Vector3.Divide(ref vector, scalar, out var actual);

        Assert.That(expectedVector, Is.EqualTo(vector));
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DotTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        float expected = System.Numerics.Vector3.Dot(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right));
        float actual = Vector3.Dot(left, right);

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void DotByRefTest()
    {
        var left = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        float expected = System.Numerics.Vector3.Dot(
            TestUtils.ConvertToSystemVec3(left), TestUtils.ConvertToSystemVec3(right));

        Vector3.Dot(ref left, ref right, out float actual);

        Assert.That(leftExpected, Is.EqualTo(left));
        Assert.That(rightExpected, Is.EqualTo(right));
        Assert.That(expected, Is.EqualTo(actual));
    }
}
