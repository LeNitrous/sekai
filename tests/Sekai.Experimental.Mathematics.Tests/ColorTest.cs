// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Experimental.Mathematics.Tests;

public class ColorTest
{
    [Test]
    public void TestRGB2HSVConversion()
    {
        Assert.That(ColorHSV.FromColor(new Color4(1, 0, 0.8f, 1)), Is.EqualTo(new ColorHSV(312, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Black), Is.EqualTo(new ColorHSV(0, 0, 0, 1)));
        Assert.That(ColorHSV.FromColor(Color.White), Is.EqualTo(new ColorHSV(0, 0, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Red), Is.EqualTo(new ColorHSV(0, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Lime), Is.EqualTo(new ColorHSV(120, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Blue), Is.EqualTo(new ColorHSV(240, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Yellow), Is.EqualTo(new ColorHSV(60, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Cyan), Is.EqualTo(new ColorHSV(180, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Magenta), Is.EqualTo(new ColorHSV(300, 1, 1, 1)));
        Assert.That(ColorHSV.FromColor(Color.Silver), Is.EqualTo(new ColorHSV(0, 0, 0.7529412f, 1)));
        Assert.That(ColorHSV.FromColor(Color.Gray), Is.EqualTo(new ColorHSV(0, 0, 0.5019608f, 1)));
        Assert.That(ColorHSV.FromColor(Color.Maroon), Is.EqualTo(new ColorHSV(0, 1, 0.5019608f, 1)));
    }

    [Test]
    public void TestHSV2RGBConversion()
    {
        Assert.That(ColorHSV.FromColor(Color.Black).ToColor(), Is.EqualTo(Color.Black.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.White).ToColor(), Is.EqualTo(Color.White.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.Red).ToColor(), Is.EqualTo(Color.Red.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.Lime).ToColor(), Is.EqualTo(Color.Lime.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.Blue).ToColor(), Is.EqualTo(Color.Blue.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.Silver).ToColor(), Is.EqualTo(Color.Silver.ToColor4()));
        Assert.That(ColorHSV.FromColor(Color.Maroon).ToColor(), Is.EqualTo(Color.Maroon.ToColor4()));
        Assert.That(ColorHSV.FromColor(new Color(184, 209, 219, 255)).ToColor().ToRgba(), Is.EqualTo(new Color(184, 209, 219, 255).ToRgba()));
    }
}
