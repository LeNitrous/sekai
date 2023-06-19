// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Sekai.Mathematics;
/// <summary>
/// Represents a color in the form of Hue, Saturation, Value, Alpha.
/// </summary>
[DataContract]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ColorHSV : IEquatable<ColorHSV>, IFormattable
{
    private const string to_string_format = "Hue:{0} Saturation:{1} Value:{2} Alpha:{3}";

    /// <summary>
    /// The Hue of the color.
    /// </summary>
    [DataMember]
    public float H;

    /// <summary>
    /// The Saturation of the color.
    /// </summary>
    [DataMember]
    public float S;

    /// <summary>
    /// The Value of the color.
    /// </summary>
    [DataMember]
    public float V;

    /// <summary>
    /// The alpha component of the color.
    /// </summary>
    [DataMember]
    public float A;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorHSV"/> struct.
    /// </summary>
    /// <param name="h">The h.</param>
    /// <param name="s">The s.</param>
    /// <param name="v">The v.</param>
    /// <param name="a">A.</param>
    public ColorHSV(float h, float s, float v, float a)
    {
        H = h;
        S = s;
        V = v;
        A = a;
    }

    /// <summary>
    /// Converts the color into a three component vector.
    /// </summary>
    /// <returns>A three component vector containing the red, green, and blue components of the color.</returns>
    public Color4 ToColor()
    {
        float hdiv = H / 60;
        int hi = (int)hdiv;
        float f = hdiv - hi;

        float p = V * (1 - S);
        float q = V * (1 - (S * f));
        float t = V * (1 - (S * (1 - f)));

        return hi switch
        {
            0 => new Color4(V, t, p, A),
            1 => new Color4(q, V, p, A),
            2 => new Color4(p, V, t, A),
            3 => new Color4(p, q, V, A),
            4 => new Color4(t, p, V, A),
            _ => new Color4(V, p, q, A),
        };
    }

    /// <summary>
    /// Converts the color into a HSV color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>A HSV color</returns>
    public static ColorHSV FromColor(Color4 color)
    {
        float max = MathF.Max(color.R, MathF.Max(color.G, color.B));
        float min = MathF.Min(color.R, MathF.Min(color.G, color.B));

        float delta = max - min;
        float h = 0.0f;

        if (delta > 0.0f)
        {
            if (color.R >= max)
                h = (color.G - color.B) / delta;
            else if (color.G >= max)
                h = (color.B - color.R) / delta + 2.0f;
            else
                h = (color.R - color.G) / delta + 4.0f;
            h *= 60.0f;

            if (h < 0)
                h += 360f;
        }

        float s = MathUtil.IsZero(max) ? 0.0f : delta / max;

        return new ColorHSV(h, s, max, color.A);
    }

    /// <inheritdoc/>
    public bool Equals(ColorHSV other)
    {
        return other.H.Equals(H) && other.S.Equals(S) && other.V.Equals(V) && other.A.Equals(A);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != typeof(ColorHSV)) return false;
        return Equals((ColorHSV)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int result = H.GetHashCode();
            result = (result * 397) ^ S.GetHashCode();
            result = (result * 397) ^ V.GetHashCode();
            result = (result * 397) ^ A.GetHashCode();
            return result;
        }
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(string format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
        return string.Format(formatProvider, to_string_format, H, S, V, A);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format == null)
            return ToString(formatProvider ?? CultureInfo.CurrentCulture);

        return string.Format(formatProvider, to_string_format,
                             H.ToString(format, formatProvider),
                             S.ToString(format, formatProvider),
                             V.ToString(format, formatProvider),
                             A.ToString(format, formatProvider));
    }

    public static bool operator ==(ColorHSV left, ColorHSV right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ColorHSV left, ColorHSV right)
    {
        return !(left == right);
    }
}
