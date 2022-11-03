// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
//
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Globalization;

namespace Sekai.Experimental.Mathematics;

public struct AngleSingle : IComparable, IComparable<AngleSingle>, IEquatable<AngleSingle>, IFormattable
{
    /// <summary>
    /// A value that specifies the size of a single degree.
    /// </summary>
    public const float DEGREE = 0.002777777777777778f;

    /// <summary>
    /// A value that specifies the size of a single minute.
    /// </summary>
    public const float MINUTE = 0.000046296296296296f;

    /// <summary>
    /// A value that specifies the size of a single second.
    /// </summary>
    public const float SECOND = 0.000000771604938272f;

    /// <summary>
    /// A value that specifies the size of a single radian.
    /// </summary>
    public const float RADIAN = 0.159154943091895336f;

    /// <summary>
    /// A value that specifies the size of a single milliradian.
    /// </summary>
    public const float MILLIRADIAN = 0.0001591549431f;

    /// <summary>
    /// A value that specifies the size of a single gradian.
    /// </summary>
    public const float GRADIAN = 0.0025f;

    /// <summary>
    /// Initializes a new instance of the <see cref="AngleSingle"/> struct with the
    /// given unit dependant angle and unit type.
    /// </summary>
    /// <param name="angle">A unit dependant measure of the angle.</param>
    /// <param name="type">The type of unit the angle argument is.</param>
    public AngleSingle(float angle, AngleType type)
    {
        Radians = type switch
        {
            AngleType.Revolution => MathUtil.RevolutionsToRadians(angle),
            AngleType.Degree => MathUtil.DegreesToRadians(angle),
            AngleType.Radian => angle,
            AngleType.Gradian => MathUtil.GradiansToRadians(angle),
            _ => 0.0f,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AngleSingle"/> struct using the
    /// arc length formula (θ = s/r).
    /// </summary>
    /// <param name="arcLength">The measure of the arc.</param>
    /// <param name="radius">The radius of the circle.</param>
    public AngleSingle(float arcLength, float radius)
    {
        Radians = arcLength / radius;
    }

    /// <summary>
    /// Wraps this Stride.Core.Mathematics.AngleSingle to be in the range [π, -π].
    /// </summary>
    public void Wrap()
    {
        float newangle = MathF.IEEERemainder(Radians, MathUtil.TWO_PI);

        if (newangle <= -MathUtil.PI)
            newangle += MathUtil.TWO_PI;
        else if (newangle > MathUtil.PI)
            newangle -= MathUtil.TWO_PI;

        Radians = newangle;
    }

    /// <summary>
    /// Wraps this Stride.Core.Mathematics.AngleSingle to be in the range [0, 2π).
    /// </summary>
    public void WrapPositive()
    {
        float newangle = Radians % MathUtil.TWO_PI;

        if (newangle < 0.0)
            newangle += MathUtil.TWO_PI;

        Radians = newangle;
    }

    /// <summary>
    /// Gets or sets the total number of revolutions this Stride.Core.Mathematics.AngleSingle represents.
    /// </summary>
    public float Revolutions
    {
        get => MathUtil.RadiansToRevolutions(Radians);
        set => Radians = MathUtil.RevolutionsToRadians(value);
    }

    /// <summary>
    /// Gets or sets the total number of degrees this Stride.Core.Mathematics.AngleSingle represents.
    /// </summary>
    public float Degrees
    {
        get => MathUtil.RadiansToDegrees(Radians);
        set => Radians = MathUtil.DegreesToRadians(value);
    }

    /// <summary>
    /// Gets or sets the minutes component of the degrees this Stride.Core.Mathematics.AngleSingle represents.
    /// When setting the minutes, if the value is in the range (-60, 60) the whole degrees are
    /// not changed; otherwise, the whole degrees may be changed. Fractional values may set
    /// the seconds component.
    /// </summary>
    public float Minutes
    {
        get
        {
            float degrees = MathUtil.RadiansToDegrees(Radians);

            if (degrees < 0)
            {
                float degreesfloor = MathF.Ceiling(degrees);
                return (degrees - degreesfloor) * 60.0f;
            }
            else
            {
                float degreesfloor = MathF.Floor(degrees);
                return (degrees - degreesfloor) * 60.0f;
            }
        }
        set
        {
            float degrees = MathUtil.RadiansToDegrees(Radians);
            float degreesfloor = MathF.Floor(degrees);

            degreesfloor += value / 60.0f;
            Radians = MathUtil.DegreesToRadians(degreesfloor);
        }
    }

    /// <summary>
    /// Gets or sets the seconds of the degrees this Stride.Core.Mathematics.AngleSingle represents.
    /// When setting te seconds, if the value is in the range (-60, 60) the whole minutes
    /// or whole degrees are not changed; otherwise, the whole minutes or whole degrees
    /// may be changed.
    /// </summary>
    public float Seconds
    {
        get
        {
            float degrees = MathUtil.RadiansToDegrees(Radians);

            if (degrees < 0)
            {
                float degreesfloor = MathF.Ceiling(degrees);

                float minutes = (degrees - degreesfloor) * 60.0f;
                float minutesfloor = MathF.Ceiling(minutes);

                return (minutes - minutesfloor) * 60.0f;
            }
            else
            {
                float degreesfloor = MathF.Floor(degrees);

                float minutes = (degrees - degreesfloor) * 60.0f;
                float minutesfloor = MathF.Floor(minutes);

                return (minutes - minutesfloor) * 60.0f;
            }
        }
        set
        {
            float degrees = MathUtil.RadiansToDegrees(Radians);
            float degreesfloor = MathF.Floor(degrees);

            float minutes = (degrees - degreesfloor) * 60.0f;
            float minutesfloor = MathF.Floor(minutes);

            minutesfloor += value / 60.0f;
            degreesfloor += minutesfloor / 60.0f;
            Radians = MathUtil.DegreesToRadians(degreesfloor);
        }
    }

    /// <summary>
    /// Gets or sets the total number of radians this Stride.Core.Mathematics.AngleSingle represents.
    /// </summary>
    public float Radians { get; set; }

    /// <summary>
    /// Gets or sets the total number of milliradians this Stride.Core.Mathematics.AngleSingle represents.
    /// One milliradian is equal to 1/(2000π).
    /// </summary>
    public float Milliradians
    {
        get => Radians / (MILLIRADIAN * MathUtil.TWO_PI);
        set => Radians = value * (MILLIRADIAN * MathUtil.TWO_PI);
    }

    /// <summary>
    /// Gets or sets the total number of gradians this Stride.Core.Mathematics.AngleSingle represents.
    /// </summary>
    public float Gradians
    {
        get => MathUtil.RadiansToGradians(Radians);
        set => Radians = MathUtil.GradiansToRadians(value);
    }

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is a right angle (i.e. 90° or π/2).
    /// </summary>
    public bool IsRight => Radians == MathUtil.PI_OVER_TWO;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is a straight angle (i.e. 180° or π).
    /// </summary>
    public bool IsStraight => Radians == MathUtil.PI;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is a full rotation angle (i.e. 360° or 2π).
    /// </summary>
    public bool IsFullRotation => Radians == MathUtil.TWO_PI;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is an oblique angle (i.e. is not 90° or a multiple of 90°).
    /// </summary>
    public bool IsOblique => WrapPositive(this).Radians != MathUtil.PI_OVER_TWO;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is an acute angle (i.e. less than 90° but greater than 0°).
    /// </summary>
    public bool IsAcute => Radians is > (float)0.0 and < MathUtil.PI_OVER_TWO;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is an obtuse angle (i.e. greater than 90° but less than 180°).
    /// </summary>
    public bool IsObtuse => Radians is > MathUtil.PI_OVER_TWO and < MathUtil.PI;

    /// <summary>
    /// Gets a System.Boolean that determines whether this Stride.Core.Mathematics.Angle
    /// is a reflex angle (i.e. greater than 180° but less than 360°).
    /// </summary>
    public bool IsReflex => Radians is > MathUtil.PI and < MathUtil.TWO_PI;

    /// <summary>
    /// Gets a Stride.Core.Mathematics.AngleSingle instance that complements this angle (i.e. the two angles add to 90°).
    /// </summary>
    public AngleSingle Complement => new(MathUtil.PI_OVER_TWO - Radians, AngleType.Radian);

    /// <summary>
    /// Gets a Stride.Core.Mathematics.AngleSingle instance that supplements this angle (i.e. the two angles add to 180°).
    /// </summary>
    public AngleSingle Supplement => new(MathUtil.PI - Radians, AngleType.Radian);

    /// <summary>
    /// Wraps the Stride.Core.Mathematics.AngleSingle given in the value argument to be in the range [π, -π].
    /// </summary>
    /// <param name="value">A Stride.Core.Mathematics.AngleSingle to wrap.</param>
    /// <returns>The Stride.Core.Mathematics.AngleSingle that is wrapped.</returns>
    public static AngleSingle Wrap(AngleSingle value)
    {
        value.Wrap();
        return value;
    }

    /// <summary>
    /// Wraps the Stride.Core.Mathematics.AngleSingle given in the value argument to be in the range [0, 2π).
    /// </summary>
    /// <param name="value">A Stride.Core.Mathematics.AngleSingle to wrap.</param>
    /// <returns>The Stride.Core.Mathematics.AngleSingle that is wrapped.</returns>
    public static AngleSingle WrapPositive(AngleSingle value)
    {
        value.WrapPositive();
        return value;
    }

    /// <summary>
    /// Compares two Stride.Core.Mathematics.AngleSingle instances and returns the smaller angle.
    /// </summary>
    /// <param name="left">The first Stride.Core.Mathematics.AngleSingle instance to compare.</param>
    /// <param name="right">The second Stride.Core.Mathematics.AngleSingle instance to compare.</param>
    /// <returns>The smaller of the two given Stride.Core.Mathematics.AngleSingle instances.</returns>
    public static AngleSingle Min(AngleSingle left, AngleSingle right)
    {
        if (left.Radians < right.Radians)
            return left;

        return right;
    }

    /// <summary>
    /// Compares two Stride.Core.Mathematics.AngleSingle instances and returns the greater angle.
    /// </summary>
    /// <param name="left">The first Stride.Core.Mathematics.AngleSingle instance to compare.</param>
    /// <param name="right">The second Stride.Core.Mathematics.AngleSingle instance to compare.</param>
    /// <returns>The greater of the two given Stride.Core.Mathematics.AngleSingle instances.</returns>
    public static AngleSingle Max(AngleSingle left, AngleSingle right)
    {
        if (left.Radians > right.Radians)
            return left;

        return right;
    }

    /// <summary>
    /// Adds two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to add.</param>
    /// <param name="right">The second object to add.</param>
    /// <returns>The value of the two objects added together.</returns>
    public static AngleSingle Add(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians + right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Subtracts two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to subtract.</param>
    /// <param name="right">The second object to subtract.</param>
    /// <returns>The value of the two objects subtracted.</returns>
    public static AngleSingle Subtract(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians - right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Multiplies two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to multiply.</param>
    /// <param name="right">The second object to multiply.</param>
    /// <returns>The value of the two objects multiplied together.</returns>
    public static AngleSingle Multiply(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians * right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Divides two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The numerator object.</param>
    /// <param name="right">The denominator object.</param>
    /// <returns>The value of the two objects divided.</returns>
    public static AngleSingle Divide(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians / right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Gets a new Stride.Core.Mathematics.AngleSingle instance that represents the zero angle (i.e. 0°).
    /// </summary>
    public static AngleSingle ZeroAngle => new(0.0f, AngleType.Radian);

    /// <summary>
    /// Gets a new Stride.Core.Mathematics.AngleSingle instance that represents the right angle (i.e. 90° or π/2).
    /// </summary>
    public static AngleSingle RightAngle => new(MathUtil.PI_OVER_TWO, AngleType.Radian);

    /// <summary>
    /// Gets a new Stride.Core.Mathematics.AngleSingle instance that represents the straight angle (i.e. 180° or π).
    /// </summary>
    public static AngleSingle StraightAngle => new(MathUtil.PI, AngleType.Radian);

    /// <summary>
    /// Gets a new Stride.Core.Mathematics.AngleSingle instance that represents the full rotation angle (i.e. 360° or 2π).
    /// </summary>
    public static AngleSingle FullRotationAngle => new(MathUtil.TWO_PI, AngleType.Radian);

    /// <summary>
    /// Returns a System.Boolean that indicates whether the values of two Stride.Core.Mathematics.Angle
    /// objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if the left and right parameters have the same value; otherwise, false.</returns>
    public static bool operator ==(AngleSingle left, AngleSingle right)
    {
        return left.Radians == right.Radians;
    }

    /// <summary>
    /// Returns a System.Boolean that indicates whether the values of two Stride.Core.Mathematics.Angle
    /// objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if the left and right parameters do not have the same value; otherwise, false.</returns>
    public static bool operator !=(AngleSingle left, AngleSingle right)
    {
        return left.Radians != right.Radians;
    }

    /// <summary>
    /// Returns a System.Boolean that indicates whether a Stride.Core.Mathematics.Angle
    /// object is less than another Stride.Core.Mathematics.AngleSingle object.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if left is less than right; otherwise, false.</returns>
    public static bool operator <(AngleSingle left, AngleSingle right)
    {
        return left.Radians < right.Radians;
    }

    /// <summary>
    /// Returns a System.Boolean that indicates whether a Stride.Core.Mathematics.Angle
    /// object is greater than another Stride.Core.Mathematics.AngleSingle object.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if left is greater than right; otherwise, false.</returns>
    public static bool operator >(AngleSingle left, AngleSingle right)
    {
        return left.Radians > right.Radians;
    }

    /// <summary>
    /// Returns a System.Boolean that indicates whether a Stride.Core.Mathematics.Angle
    /// object is less than or equal to another Stride.Core.Mathematics.AngleSingle object.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if left is less than or equal to right; otherwise, false.</returns>
    public static bool operator <=(AngleSingle left, AngleSingle right)
    {
        return left.Radians <= right.Radians;
    }

    /// <summary>
    /// Returns a System.Boolean that indicates whether a Stride.Core.Mathematics.Angle
    /// object is greater than or equal to another Stride.Core.Mathematics.AngleSingle object.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>True if left is greater than or equal to right; otherwise, false.</returns>
    public static bool operator >=(AngleSingle left, AngleSingle right)
    {
        return left.Radians >= right.Radians;
    }

    /// <summary>
    /// Returns the value of the Stride.Core.Mathematics.AngleSingle operand. (The sign of
    /// the operand is unchanged.)
    /// </summary>
    /// <param name="value">A Stride.Core.Mathematics.AngleSingle object.</param>
    /// <returns>The value of the value parameter.</returns>
    public static AngleSingle operator +(AngleSingle value)
    {
        return value;
    }

    /// <summary>
    /// Returns the the negated value of the Stride.Core.Mathematics.AngleSingle operand.
    /// </summary>
    /// <param name="value">A Stride.Core.Mathematics.AngleSingle object.</param>
    /// <returns>The negated value of the value parameter.</returns>
    public static AngleSingle operator -(AngleSingle value)
    {
        return new AngleSingle(-value.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Adds two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to add.</param>
    /// <param name="right">The second object to add.</param>
    /// <returns>The value of the two objects added together.</returns>
    public static AngleSingle operator +(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians + right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Subtracts two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to subtract</param>
    /// <param name="right">The second object to subtract.</param>
    /// <returns>The value of the two objects subtracted.</returns>
    public static AngleSingle operator -(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians - right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Multiplies two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The first object to multiply.</param>
    /// <param name="right">The second object to multiply.</param>
    /// <returns>The value of the two objects multiplied together.</returns>
    public static AngleSingle operator *(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians * right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Divides two Stride.Core.Mathematics.AngleSingle objects and returns the result.
    /// </summary>
    /// <param name="left">The numerator object.</param>
    /// <param name="right">The denominator object.</param>
    /// <returns>The value of the two objects divided.</returns>
    public static AngleSingle operator /(AngleSingle left, AngleSingle right)
    {
        return new AngleSingle(left.Radians / right.Radians, AngleType.Radian);
    }

    /// <summary>
    /// Compares this instance to a specified object and returns an integer that
    /// indicates whether the value of this instance is less than, equal to, or greater
    /// than the value of the specified object.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relationship of the current instance
    /// to the obj parameter. If the value is less than zero, the current instance
    /// is less than the other. If the value is zero, the current instance is equal
    /// to the other. If the value is greater than zero, the current instance is
    /// greater than the other.
    /// </returns>
    public int CompareTo(object other)
    {
        if (other == null)
            return 1;

        if (other is not AngleSingle)
            throw new ArgumentException("Argument must be of type Angle.", nameof(other));

        float radians = ((AngleSingle)other).Radians;

        if (this.Radians > radians)
            return 1;

        if (this.Radians < radians)
            return -1;

        return 0;
    }

    /// <summary>
    /// Compares this instance to a second Stride.Core.Mathematics.AngleSingle and returns
    /// an integer that indicates whether the value of this instance is less than,
    /// equal to, or greater than the value of the specified object.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relationship of the current instance
    /// to the obj parameter. If the value is less than zero, the current instance
    /// is less than the other. If the value is zero, the current instance is equal
    /// to the other. If the value is greater than zero, the current instance is
    /// greater than the other.
    /// </returns>
    public int CompareTo(AngleSingle other)
    {
        if (this.Radians > other.Radians)
            return 1;

        if (this.Radians < other.Radians)
            return -1;

        return 0;
    }

    /// <summary>
    /// Returns a value that indicates whether the current instance and a specified
    /// Stride.Core.Mathematics.AngleSingle object have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>
    /// Returns true if this Stride.Core.Mathematics.AngleSingle object and another have the same value;
    /// otherwise, false.
    /// </returns>
    public bool Equals(AngleSingle other)
    {
        return this == other;
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, MathUtil.RadiansToDegrees(Radians).ToString("0.##°"));
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
        if (format == null)
            return ToString();

        return string.Format(CultureInfo.CurrentCulture, "{0}°", MathUtil.RadiansToDegrees(Radians).ToString(format, CultureInfo.CurrentCulture));
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
        return string.Format(formatProvider, MathUtil.RadiansToDegrees(Radians).ToString("0.##°"));
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

        return string.Format(formatProvider, "{0}°", MathUtil.RadiansToDegrees(Radians).ToString(format, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Returns a hash code for this Stride.Core.Mathematics.AngleSingle instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return (int)(BitConverter.DoubleToInt64Bits(Radians) % int.MaxValue);
    }

    /// <summary>
    /// Returns a value that indicates whether the current instance and a specified
    /// object have the same value.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    /// Returns true if the obj parameter is a Stride.Core.Mathematics.AngleSingle object or a type
    /// capable of implicit conversion to a Stride.Core.Mathematics.AngleSingle value, and
    /// its value is equal to the value of the current Stride.Core.Mathematics.Angle
    /// object; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return (obj is AngleSingle single) && (this == single);
    }
}
