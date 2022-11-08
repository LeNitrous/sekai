// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Sekai.Experimental.Mathematics.Tests;
internal class QuaternionTests
{
    private static readonly Random random = new();

    [Test]
    public void QuaternionReadonlyFieldsTest()
    {
        // there is now concievable way we needed this
        // but we need to make sure it's size in bytes is exactly what
        // will be marshaled in.
        Assert.That(Marshal.SizeOf<Quaternion>(), Is.EqualTo(Quaternion.SizeInBytes));
        Assert.That(Quaternion.Identity, Is.EqualTo(new Quaternion(0f, 0f, 0f, 1f)));
    }

    [Test]
    public void QuaternionConstructorTest()
    {
        var expectedValue = new Quaternion(0f, 0f, 0f, 1f);
        Assert.That(Quaternion.Identity, Is.EqualTo(expectedValue));
    }

    [Test]
    public void QuaternionAdditionTest()
    {
        var left = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Add(
            TestUtils.ConvertToSystemQuaternion(left), TestUtils.ConvertToSystemQuaternion(right)));
        var actual = left + right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void QuaternionAdditionByRefTest()
    {
        var left = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Add(
            TestUtils.ConvertToSystemQuaternion(left), TestUtils.ConvertToSystemQuaternion(right)));

        Quaternion.Add(ref left, ref right, out var result);

        Assert.That(left, Is.EqualTo(leftExpected));
        Assert.That(right, Is.EqualTo(rightExpected));
        Assert.That(result, Is.EqualTo(resultExpected));
    }

    [Test]
    public void QuaternionSubtractionTest()
    {
        var left = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Subtract(
            TestUtils.ConvertToSystemQuaternion(left), TestUtils.ConvertToSystemQuaternion(right)));
        var actual = left - right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void QuaternionSubtractionByRefTest()
    {
        var left = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());

        // copy the values so we have a reference on what they looked like before.
        var leftExpected = left;
        var rightExpected = right;
        var resultExpected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Subtract(
            TestUtils.ConvertToSystemQuaternion(left), TestUtils.ConvertToSystemQuaternion(right)));

        Quaternion.Subtract(ref left, ref right, out var result);

        Assert.That(left, Is.EqualTo(leftExpected));
        Assert.That(right, Is.EqualTo(rightExpected));
        Assert.That(result, Is.EqualTo(resultExpected));
    }

    [Test]
    public void QuaternionMultiplicationTest()
    {
        var left = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var right = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Multiply(
            TestUtils.ConvertToSystemQuaternion(left), TestUtils.ConvertToSystemQuaternion(right)));
        var actual = left * right;

        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void MultiplicationByRefTest()
    {
        var quaternion = new Quaternion(random.NextSingle(), random.NextSingle(), random.NextSingle(), random.NextSingle());
        var expectedQuaternion = quaternion;
        float scalar = random.NextSingle();

        var expected = TestUtils.ConvertFromSystemQuaternion(System.Numerics.Quaternion.Multiply(
            TestUtils.ConvertToSystemQuaternion(quaternion), scalar));

        Quaternion.Multiply(ref quaternion, scalar, out var result);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(quaternion, Is.EqualTo(expectedQuaternion));
    }
}
