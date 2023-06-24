// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for performing various arithmetic operators on colors.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IColorOperators<TSelf> : IAdditionOperators<TSelf, TSelf, TSelf>, ISubtractionOperators<TSelf, TSelf, TSelf>, IMultiplyOperators<TSelf, TSelf, TSelf>
    where TSelf : struct, IColorOperators<TSelf>, IAdditionOperators<TSelf, TSelf, TSelf>, ISubtractionOperators<TSelf, TSelf, TSelf>, IMultiplyOperators<TSelf, TSelf, TSelf>
{
    /// <summary>
    /// Adds two colors.
    /// </summary>
    /// <param name="left">The first color to add.</param>
    /// <param name="right">The second color to add.</param>
    /// <param name="result">When the method completes, completes the sum of the two colors.</param>
    static abstract void Add(ref TSelf left, ref TSelf right, out TSelf result);

    /// <summary>
    /// Adds two colors.
    /// </summary>
    /// <param name="left">The first color to add.</param>
    /// <param name="right">The second color to add.</param>
    /// <returns>The sum of the two colors.</returns>
    static abstract TSelf Add(TSelf left, TSelf right);

    /// <summary>
    /// Subtracts two colors.
    /// </summary>
    /// <param name="left">The first color to subtract.</param>
    /// <param name="right">The second color to subtract.</param>
    /// <param name="result">WHen the method completes, contains the difference of the two colors.</param>
    static abstract void Subtract(ref TSelf left, ref TSelf right, out TSelf result);

    /// <summary>
    /// Subtracts two colors.
    /// </summary>
    /// <param name="left">The first color to subtract.</param>
    /// <param name="right">The second color to subtract</param>
    /// <returns>The difference of the two colors.</returns>
    static abstract TSelf Subtract(TSelf left, TSelf right);

    /// <summary>
    /// Modulates two colors.
    /// </summary>
    /// <param name="left">The first color to modulate.</param>
    /// <param name="right">The second color to modulate.</param>
    /// <param name="result">When the method completes, contains the modulated color.</param>
    static abstract void Modulate(ref TSelf left, ref TSelf right, out TSelf result);

    // <summary>
    /// Modulates two colors.
    /// </summary>
    /// <param name="left">The first color to modulate.</param>
    /// <param name="right">The second color to modulate.</param>
    /// <returns>The modulated color.</returns>
    static abstract TSelf Modulate(TSelf left, TSelf right);

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="value">The color to scale.</param>
    /// <param name="scale">The amount by which to scale.</param>
    /// <param name="result">When the method completes, contains the scaled color.</param>
    static abstract void Scale(ref TSelf value, float scale, out TSelf result);

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="value">The color to scale.</param>
    /// <param name="scale">The amount by which to scale.</param>
    /// <returns>The scaled color.</returns>
    static abstract TSelf Scale(TSelf value, float scale);

    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <param name="result">When the method completes, contains the negated color.</param>
    static abstract void Negate(ref TSelf value, out TSelf result);

    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <returns>The negated color.</returns>
    static abstract TSelf Negate(TSelf value);

    /// <summary>
    /// Restricts a value to be within a specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="result">When the method completes, contains the clamped value.</param>
    static abstract void Clamp(ref TSelf value, ref TSelf min, ref TSelf max, out TSelf result);

    /// <summary>
    /// Restricts a value to be within a specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    static abstract TSelf Clamp(TSelf value, TSelf min, TSelf max);

    /// <summary>
    /// Performs a linear interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the linear interpolation of the two colors.</param>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    static abstract void Lerp(ref TSelf start, ref TSelf end, float amount, out TSelf result);

    /// <summary>
    /// Performs a linear interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The linear interpolation of the two colors.</returns>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    static abstract TSelf Lerp(TSelf start, TSelf end, float amount);

    /// <summary>
    /// Performs a cubic interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the cubic interpolation of the two colors.</param>

    static abstract void SmoothStep(ref TSelf start, ref TSelf end, float amount, out TSelf result);

    /// <summary>
    /// Performs a cubic interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The cubic interpolation of the two colors.</returns>
    static abstract TSelf SmoothStep(TSelf start, TSelf end, float amount);

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <param name="result">When the method completes, contains an new color composed of the largest components of the source colors.</param>
    static abstract void Max(ref TSelf left, ref TSelf right, out TSelf result);

    /// <summary>
    /// Returns a color containing the largest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <returns>A color containing the largest components of the source colors.</returns>
    static abstract TSelf Max(TSelf left, TSelf right);

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <param name="result">When the method completes, contains an new color composed of the smallest components of the source colors.</param>
    static abstract void Min(ref TSelf left, ref TSelf right, out TSelf result);

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <returns>A color containing the smallest components of the source colors.</returns>
    static abstract TSelf Min(TSelf left, TSelf right);

    /// <summary>
    /// Adjusts the contrast of a color.
    /// </summary>
    /// <param name="value">The color whose contrast is to be adjusted.</param>
    /// <param name="contrast">The amount by which to adjust the contrast.</param>
    /// <param name="result">When the method completes, contains the adjusted color.</param>
    static abstract void AdjustContrast(ref TSelf value, float contrast, out TSelf result);

    /// <summary>
    /// Adjusts the contrast of a color.
    /// </summary>
    /// <param name="value">The color whose contrast is to be adjusted.</param>
    /// <param name="contrast">The amount by which to adjust the contrast.</param>
    /// <returns>The adjusted color.</returns>
    static abstract TSelf AdjustContrast(TSelf value, float contrast);

    /// <summary>
    /// Adjusts the saturation of a color.
    /// </summary>
    /// <param name="value">The color whose saturation is to be adjusted.</param>
    /// <param name="saturation">The amount by which to adjust the saturation.</param>
    /// <param name="result">When the method completes, contains the adjusted color.</param>
    static abstract void AdjustSaturation(ref TSelf value, float saturation, out TSelf result);

    /// <summary>
    /// Adjusts the saturation of a color.
    /// </summary>
    /// <param name="value">The color whose saturation is to be adjusted.</param>
    /// <param name="saturation">The amount by which to adjust the saturation.</param>
    /// <returns>The adjusted color.</returns>
    static abstract TSelf AdjustSaturation(TSelf value, float saturation);

    /// <summary>
    /// Assert a color (return it unchanged).
    /// </summary>
    /// <param name="value">The color to assert (unchanged).</param>
    /// <returns>The asserted (unchanged) color.</returns>
    static abstract TSelf operator +(TSelf value);

    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <returns>A negated color.</returns>
    static abstract TSelf operator -(TSelf value);

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="scale">The factor by which to scale the color.</param>
    /// <param name="value">The color to scale.</param>
    /// <returns>The scaled color.</returns>
    static abstract TSelf operator *(TSelf value, float scale);

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="value">The factor by which to scale the color.</param>
    /// <param name="scale">The color to scale.</param>
    /// <returns>The scaled color.</returns>
    static abstract TSelf operator *(float scale, TSelf value);
}
