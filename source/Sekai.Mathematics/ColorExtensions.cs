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

namespace Sekai.Mathematics;
/// <summary>
/// A class containing extension methods for processing colors.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Indicates if the given string can be converted to an <see cref="uint"/> RGBA value using <see cref="StringToRgba"/>.
    /// </summary>
    /// <param name="stringColor">The string to convert.</param>
    /// <returns>True if the string can be converted, false otherwise.</returns>
    public static bool CanConvertStringToRgba(string? stringColor)
    {
        return stringColor?.StartsWith("#") ?? false;
    }

    /// <summary>
    /// Converts the given string to an <see cref="uint"/> RGBA value.
    /// </summary>
    /// <param name="stringColor">The string to convert.</param>
    /// <returns>The converted RGBA value.</returns>
    public static uint StringToRgba(string? stringColor)
    {
        uint intValue = 0xFF000000;
        if (stringColor != null)
        {
            if (stringColor.StartsWith("#"))
            {
                if (stringColor.Length == "#000".Length && uint.TryParse(stringColor.AsSpan(1, 3), NumberStyles.HexNumber, null, out intValue))
                {
                    intValue = ((intValue & 0x00F) << 16)
                               | ((intValue & 0x00F) << 20)
                               | ((intValue & 0x0F0) << 4)
                               | ((intValue & 0x0F0) << 8)
                               | ((intValue & 0xF00) >> 4)
                               | ((intValue & 0xF00) >> 8)
                               | (0xFF000000);
                }
                if (stringColor.Length == "#000000".Length && uint.TryParse(stringColor.AsSpan(1, 6), NumberStyles.HexNumber, null, out intValue))
                {
                    intValue = ((intValue & 0x000000FF) << 16)
                               | (intValue & 0x0000FF00)
                               | ((intValue & 0x00FF0000) >> 16)
                               | (0xFF000000);
                }
                if (stringColor.Length == "#00000000".Length && uint.TryParse(stringColor.AsSpan(1, 8), NumberStyles.HexNumber, null, out intValue))
                {
                    intValue = ((intValue & 0x000000FF) << 16)
                               | (intValue & 0x0000FF00)
                               | ((intValue & 0x00FF0000) >> 16)
                               | (intValue & 0xFF000000);
                }
            }
        }
        return intValue;
    }

    /// <summary>
    /// Converts the given RGB value to a string.
    /// </summary>
    /// <param name="value">The RGB value to convert.</param>
    /// <returns>The converted string.</returns>
    public static string RgbToString(int value)
    {
        int r = (value & 0x000000FF);
        int g = (value & 0x0000FF00) >> 8;
        int b = (value & 0x00FF0000) >> 16;
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Converts the given RGBA value to a string.
    /// </summary>
    /// <param name="value">The RGBA value to convert.</param>
    /// <returns>The converted string.</returns>
    public static string RgbaToString(int value)
    {
        int r = (value & 0x000000FF);
        int g = (value & 0x0000FF00) >> 8;
        int b = (value & 0x00FF0000) >> 16;
        long a = (value & 0xFF000000) >> 24;
        return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
    }
}
