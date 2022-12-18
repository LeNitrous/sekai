// Copyright (c) The Vignette Authors
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
using System.Numerics;
using System.Runtime.Serialization;

namespace Sekai.Mathematics;

/// <summary>
/// A representation of a sphere of values via Spherical Harmonics (SH).
/// </summary>
/// <typeparam name="TDataType">The type of data contained by the sphere</typeparam>
[DataContract]
public abstract class SphericalHarmonics<TDataType>
{
    /// <summary>
    /// The maximum order supported.
    /// </summary>
    public const int MAXIMUM_ORDER = 5;

    private int order;

    /// <summary>
    /// The order of calculation of the spherical harmonic.
    /// </summary>
    public int Order
    {
        get => order;
        internal set
        {
            if (order > 5)
                throw new NotSupportedException("Only orders inferior or equal to 5 are supported");

            order = Math.Max(1, value);
        }
    }

    /// <summary>
    /// Get the coefficients defining the spherical harmonics (the spherical coordinates x{l,m} multiplying the spherical base Y{l,m}).
    /// </summary>
    public TDataType[] Coefficients { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SphericalHarmonics{TDataType}"/> class (null, for serialization).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal SphericalHarmonics()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SphericalHarmonics{TDataType}"/> class.
    /// </summary>
    /// <param name="order">The order of the harmonics</param>
    protected SphericalHarmonics(int order)
    {
        this.order = order;
        Coefficients = new TDataType[order * order];
    }

    /// <summary>
    /// Evaluate the value of the spherical harmonics in the provided direction.
    /// </summary>
    /// <param name="direction">The direction</param>
    /// <returns>The value of the spherical harmonics in the direction</returns>
    public abstract TDataType Evaluate(Vector3 direction);

    /// <summary>
    /// Returns the coefficient x{l,m} of the spherical harmonics (the {l,m} spherical coordinate corresponding to the spherical base Y{l,m}).
    /// </summary>
    /// <param name="l">the l index of the coefficient</param>
    /// <param name="m">the m index of the coefficient</param>
    /// <returns>the value of the coefficient</returns>
    public TDataType this[int l, int m]
    {
        get
        {
            checkIndicesValidity(l, m, order);
            return Coefficients[lmToCoefficientIndex(l, m)];
        }
        set
        {
            checkIndicesValidity(l, m, order);
            Coefficients[lmToCoefficientIndex(l, m)] = value;
        }
    }

    // ReSharper disable UnusedParameter.Local
    private static void checkIndicesValidity(int l, int m, int maxOrder)
    // ReSharper restore UnusedParameter.Local
    {
        if (l > maxOrder - 1)
            throw new IndexOutOfRangeException($"'l' parameter should be between '0' and '{maxOrder - 1}' (order-1).");

        if (Math.Abs(m) > l)
            throw new IndexOutOfRangeException("'m' parameter should be between '-l' and '+l'.");
    }

    private static int lmToCoefficientIndex(int l, int m)
    {
        return l * l + l + m;
    }
}

/// <summary>
/// A spherical harmonics representation of a cubemap.
/// </summary>
public class SphericalHarmonics : SphericalHarmonics<Color3>
{
    private readonly float[] baseValues;

    private const float pi_4 = 4 * MathUtil.PI;
    private const float pi_16 = 16 * MathUtil.PI;
    private const float pi_64 = 64 * MathUtil.PI;
    private static readonly float sqrtPi = (float)Math.Sqrt(MathUtil.PI);

    /// <summary>
    /// Base coefficients for SH.
    /// </summary>
    public static readonly float[] BaseCoefficients =
    {
            (float)(1.0/(2.0*sqrtPi)),

            (float)(-Math.Sqrt(3.0/pi_4)),
            (float)(Math.Sqrt(3.0/pi_4)),
            (float)(-Math.Sqrt(3.0/pi_4)),

            (float)(Math.Sqrt(15.0/pi_4)),
            (float)(-Math.Sqrt(15.0/pi_4)),
            (float)(Math.Sqrt(5.0/pi_16)),
            (float)(-Math.Sqrt(15.0/pi_4)),
            (float)(Math.Sqrt(15.0/pi_16)),

            -(float)Math.Sqrt(70/pi_64),
            (float)Math.Sqrt(105/pi_4),
            -(float)Math.Sqrt(42/pi_64),
            (float)Math.Sqrt(7/pi_16),
            -(float)Math.Sqrt(42/pi_64),
            (float)Math.Sqrt(105/pi_16),
            -(float)Math.Sqrt(70/pi_64),

            3*(float)Math.Sqrt(35/pi_16),
            -3*(float)Math.Sqrt(70/pi_64),
            3*(float)Math.Sqrt(5/pi_16),
            -3*(float)Math.Sqrt(10/pi_64),
            (float)(1.0/(16.0*sqrtPi)),
            -3*(float)Math.Sqrt(10/pi_64),
            3*(float)Math.Sqrt(5/pi_64),
            -3*(float)Math.Sqrt(70/pi_64),
            3*(float)Math.Sqrt(35/(4*pi_64)),
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="SphericalHarmonics"/> class (null, for serialization).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal SphericalHarmonics()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SphericalHarmonics"/> class.
    /// </summary>
    /// <param name="order">The order of the harmonics</param>
    public SphericalHarmonics(int order)
        : base(order)
    {
        baseValues = new float[order * order];
    }

    /// <summary>
    /// Evaluates the color for the specified direction.
    /// </summary>
    /// <param name="direction">The direction to evaluate.</param>
    /// <returns>The color computed for this direction.</returns>
    public override Color3 Evaluate(Vector3 direction)
    {
        float x = direction.X;
        float y = direction.Y;
        float z = direction.Z;

        float x2 = x * x;
        float y2 = y * y;
        float z2 = z * z;

        float z3 = (float)Math.Pow(z, 3.0);

        float x4 = (float)Math.Pow(x, 4.0);
        float y4 = (float)Math.Pow(y, 4.0);
        float z4 = (float)Math.Pow(z, 4.0);

        //Equations based on data from: http://ppsloan.org/publications/StupidSH36.pdf
        baseValues[0] = 1 / (2 * sqrtPi);

        if (Order > 1)
        {
            baseValues[1] = -(float)Math.Sqrt(3 / pi_4) * y;
            baseValues[2] = (float)Math.Sqrt(3 / pi_4) * z;
            baseValues[3] = -(float)Math.Sqrt(3 / pi_4) * x;

            if (Order > 2)
            {
                baseValues[4] = (float)Math.Sqrt(15 / pi_4) * y * x;
                baseValues[5] = -(float)Math.Sqrt(15 / pi_4) * y * z;
                baseValues[6] = (float)Math.Sqrt(5 / pi_16) * (3 * z2 - 1);
                baseValues[7] = -(float)Math.Sqrt(15 / pi_4) * x * z;
                baseValues[8] = (float)Math.Sqrt(15 / pi_16) * (x2 - y2);

                if (Order > 3)
                {
                    baseValues[9] = -(float)Math.Sqrt(70 / pi_64) * y * (3 * x2 - y2);
                    baseValues[10] = (float)Math.Sqrt(105 / pi_4) * y * x * z;
                    baseValues[11] = -(float)Math.Sqrt(42 / pi_64) * y * (-1 + 5 * z2);
                    baseValues[12] = (float)Math.Sqrt(7 / pi_16) * (5 * z3 - 3 * z);
                    baseValues[13] = -(float)Math.Sqrt(42 / pi_64) * x * (-1 + 5 * z2);
                    baseValues[14] = (float)Math.Sqrt(105 / pi_16) * (x2 - y2) * z;
                    baseValues[15] = -(float)Math.Sqrt(70 / pi_64) * x * (x2 - 3 * y2);

                    if (Order > 4)
                    {
                        baseValues[16] = 3 * (float)Math.Sqrt(35 / pi_16) * x * y * (x2 - y2);
                        baseValues[17] = -3 * (float)Math.Sqrt(70 / pi_64) * y * z * (3 * x2 - y2);
                        baseValues[18] = 3 * (float)Math.Sqrt(5 / pi_16) * y * x * (-1 + 7 * z2);
                        baseValues[19] = -3 * (float)Math.Sqrt(10 / pi_64) * y * z * (-3 + 7 * z2);
                        baseValues[20] = (105 * z4 - 90 * z2 + 9) / (16 * sqrtPi);
                        baseValues[21] = -3 * (float)Math.Sqrt(10 / pi_64) * x * z * (-3 + 7 * z2);
                        baseValues[22] = 3 * (float)Math.Sqrt(5 / pi_64) * (x2 - y2) * (-1 + 7 * z2);
                        baseValues[23] = -3 * (float)Math.Sqrt(70 / pi_64) * x * z * (x2 - 3 * y2);
                        baseValues[24] = 3 * (float)Math.Sqrt(35 / (4 * pi_64)) * (x4 - 6 * y2 * x2 + y4);
                    }
                }
            }
        }

        var data = new Color3();

        for (int i = 0; i < baseValues.Length; i++)
            data += Coefficients[i] * baseValues[i];

        return data;
    }
}
