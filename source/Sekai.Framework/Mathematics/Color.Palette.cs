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

namespace Sekai.Framework.Mathematics;

/// <summary>
/// List of predefined <see cref="Color" />.
/// </summary>
public partial struct Color
{
    /// <summary>
    /// Transparent color.
    /// </summary>
    public static readonly Color Transparent = FromBGRA(0x00000000);

    /// <summary>
    /// AliceBlue color.
    /// </summary>
    public static readonly Color AliceBlue = FromBGRA(0xFFF0F8FF);

    /// <summary>
    /// AntiqueWhite color.
    /// </summary>
    public static readonly Color AntiqueWhite = FromBGRA(0xFFFAEBD7);

    /// <summary>
    /// Aqua color.
    /// </summary>
    public static readonly Color Aqua = FromBGRA(0xFF00FFFF);

    /// <summary>
    /// Aquamarine color.
    /// </summary>
    public static readonly Color Aquamarine = FromBGRA(0xFF7FFFD4);

    /// <summary>
    /// Azure color.
    /// </summary>
    public static readonly Color Azure = FromBGRA(0xFFF0FFFF);

    /// <summary>
    /// Beige color.
    /// </summary>
    public static readonly Color Beige = FromBGRA(0xFFF5F5DC);

    /// <summary>
    /// Bisque color.
    /// </summary>
    public static readonly Color Bisque = FromBGRA(0xFFFFE4C4);

    /// <summary>
    /// Black color.
    /// </summary>
    public static readonly Color Black = FromBGRA(0xFF000000);

    /// <summary>
    /// BlanchedAlmond color.
    /// </summary>
    public static readonly Color BlanchedAlmond = FromBGRA(0xFFFFEBCD);

    /// <summary>
    /// Blue color.
    /// </summary>
    public static readonly Color Blue = FromBGRA(0xFF0000FF);

    /// <summary>
    /// BlueViolet color.
    /// </summary>
    public static readonly Color BlueViolet = FromBGRA(0xFF8A2BE2);

    /// <summary>
    /// Brown color.
    /// </summary>
    public static readonly Color Brown = FromBGRA(0xFFA52A2A);

    /// <summary>
    /// BurlyWood color.
    /// </summary>
    public static readonly Color BurlyWood = FromBGRA(0xFFDEB887);

    /// <summary>
    /// CadetBlue color.
    /// </summary>
    public static readonly Color CadetBlue = FromBGRA(0xFF5F9EA0);

    /// <summary>
    /// Chartreuse color.
    /// </summary>
    public static readonly Color Chartreuse = FromBGRA(0xFF7FFF00);

    /// <summary>
    /// Chocolate color.
    /// </summary>
    public static readonly Color Chocolate = FromBGRA(0xFFD2691E);

    /// <summary>
    /// Coral color.
    /// </summary>
    public static readonly Color Coral = FromBGRA(0xFFFF7F50);

    /// <summary>
    /// CornflowerBlue color.
    /// </summary>
    public static readonly Color CornflowerBlue = FromBGRA(0xFF6495ED);

    /// <summary>
    /// Cornsilk color.
    /// </summary>
    public static readonly Color Cornsilk = FromBGRA(0xFFFFF8DC);

    /// <summary>
    /// Crimson color.
    /// </summary>
    public static readonly Color Crimson = FromBGRA(0xFFDC143C);

    /// <summary>
    /// Cyan color.
    /// </summary>
    public static readonly Color Cyan = FromBGRA(0xFF00FFFF);

    /// <summary>
    /// DarkBlue color.
    /// </summary>
    public static readonly Color DarkBlue = FromBGRA(0xFF00008B);

    /// <summary>
    /// DarkCyan color.
    /// </summary>
    public static readonly Color DarkCyan = FromBGRA(0xFF008B8B);

    /// <summary>
    /// DarkGoldenrod color.
    /// </summary>
    public static readonly Color DarkGoldenrod = FromBGRA(0xFFB8860B);

    /// <summary>
    /// DarkGray color.
    /// </summary>
    public static readonly Color DarkGray = FromBGRA(0xFFA9A9A9);

    /// <summary>
    /// DarkGreen color.
    /// </summary>
    public static readonly Color DarkGreen = FromBGRA(0xFF006400);

    /// <summary>
    /// DarkKhaki color.
    /// </summary>
    public static readonly Color DarkKhaki = FromBGRA(0xFFBDB76B);

    /// <summary>
    /// DarkMagenta color.
    /// </summary>
    public static readonly Color DarkMagenta = FromBGRA(0xFF8B008B);

    /// <summary>
    /// DarkOliveGreen color.
    /// </summary>
    public static readonly Color DarkOliveGreen = FromBGRA(0xFF556B2F);

    /// <summary>
    /// DarkOrange color.
    /// </summary>
    public static readonly Color DarkOrange = FromBGRA(0xFFFF8C00);

    /// <summary>
    /// DarkOrchid color.
    /// </summary>
    public static readonly Color DarkOrchid = FromBGRA(0xFF9932CC);

    /// <summary>
    /// DarkRed color.
    /// </summary>
    public static readonly Color DarkRed = FromBGRA(0xFF8B0000);

    /// <summary>
    /// DarkSalmon color.
    /// </summary>
    public static readonly Color DarkSalmon = FromBGRA(0xFFE9967A);

    /// <summary>
    /// DarkSeaGreen color.
    /// </summary>
    public static readonly Color DarkSeaGreen = FromBGRA(0xFF8FBC8B);

    /// <summary>
    /// DarkSlateBlue color.
    /// </summary>
    public static readonly Color DarkSlateBlue = FromBGRA(0xFF483D8B);

    /// <summary>
    /// DarkSlateGray color.
    /// </summary>
    public static readonly Color DarkSlateGray = FromBGRA(0xFF2F4F4F);

    /// <summary>
    /// DarkTurquoise color.
    /// </summary>
    public static readonly Color DarkTurquoise = FromBGRA(0xFF00CED1);

    /// <summary>
    /// DarkViolet color.
    /// </summary>
    public static readonly Color DarkViolet = FromBGRA(0xFF9400D3);

    /// <summary>
    /// DeepPink color.
    /// </summary>
    public static readonly Color DeepPink = FromBGRA(0xFFFF1493);

    /// <summary>
    /// DeepSkyBlue color.
    /// </summary>
    public static readonly Color DeepSkyBlue = FromBGRA(0xFF00BFFF);

    /// <summary>
    /// DimGray color.
    /// </summary>
    public static readonly Color DimGray = FromBGRA(0xFF696969);

    /// <summary>
    /// VeryDimGray color.
    /// </summary>
    public static readonly Color VeryDimGray = FromBGRA(0xFF404040);

    /// <summary>
    /// DodgerBlue color.
    /// </summary>
    public static readonly Color DodgerBlue = FromBGRA(0xFF1E90FF);

    /// <summary>
    /// Firebrick color.
    /// </summary>
    public static readonly Color Firebrick = FromBGRA(0xFFB22222);

    /// <summary>
    /// FloralWhite color.
    /// </summary>
    public static readonly Color FloralWhite = FromBGRA(0xFFFFFAF0);

    /// <summary>
    /// ForestGreen color.
    /// </summary>
    public static readonly Color ForestGreen = FromBGRA(0xFF228B22);

    /// <summary>
    /// Fuchsia color.
    /// </summary>
    public static readonly Color Fuchsia = FromBGRA(0xFFFF00FF);

    /// <summary>
    /// Gainsboro color.
    /// </summary>
    public static readonly Color Gainsboro = FromBGRA(0xFFDCDCDC);

    /// <summary>
    /// GhostWhite color.
    /// </summary>
    public static readonly Color GhostWhite = FromBGRA(0xFFF8F8FF);

    /// <summary>
    /// Gold color.
    /// </summary>
    public static readonly Color Gold = FromBGRA(0xFFFFD700);

    /// <summary>
    /// Goldenrod color.
    /// </summary>
    public static readonly Color Goldenrod = FromBGRA(0xFFDAA520);

    /// <summary>
    /// Gray color.
    /// </summary>
    public static readonly Color Gray = FromBGRA(0xFF808080);

    /// <summary>
    /// Green color.
    /// </summary>
    public static readonly Color Green = FromBGRA(0xFF008000);

    /// <summary>
    /// GreenYellow color.
    /// </summary>
    public static readonly Color GreenYellow = FromBGRA(0xFFADFF2F);

    /// <summary>
    /// Honeydew color.
    /// </summary>
    public static readonly Color Honeydew = FromBGRA(0xFFF0FFF0);

    /// <summary>
    /// HotPink color.
    /// </summary>
    public static readonly Color HotPink = FromBGRA(0xFFFF69B4);

    /// <summary>
    /// IndianRed color.
    /// </summary>
    public static readonly Color IndianRed = FromBGRA(0xFFCD5C5C);

    /// <summary>
    /// Indigo color.
    /// </summary>
    public static readonly Color Indigo = FromBGRA(0xFF4B0082);

    /// <summary>
    /// Ivory color.
    /// </summary>
    public static readonly Color Ivory = FromBGRA(0xFFFFFFF0);

    /// <summary>
    /// Khaki color.
    /// </summary>
    public static readonly Color Khaki = FromBGRA(0xFFF0E68C);

    /// <summary>
    /// Lavender color.
    /// </summary>
    public static readonly Color Lavender = FromBGRA(0xFFE6E6FA);

    /// <summary>
    /// LavenderBlush color.
    /// </summary>
    public static readonly Color LavenderBlush = FromBGRA(0xFFFFF0F5);

    /// <summary>
    /// LawnGreen color.
    /// </summary>
    public static readonly Color LawnGreen = FromBGRA(0xFF7CFC00);

    /// <summary>
    /// LemonChiffon color.
    /// </summary>
    public static readonly Color LemonChiffon = FromBGRA(0xFFFFFACD);

    /// <summary>
    /// LightBlue color.
    /// </summary>
    public static readonly Color LightBlue = FromBGRA(0xFFADD8E6);

    /// <summary>
    /// LightCoral color.
    /// </summary>
    public static readonly Color LightCoral = FromBGRA(0xFFF08080);

    /// <summary>
    /// LightCyan color.
    /// </summary>
    public static readonly Color LightCyan = FromBGRA(0xFFE0FFFF);

    /// <summary>
    /// LightGoldenrodYellow color.
    /// </summary>
    public static readonly Color LightGoldenrodYellow = FromBGRA(0xFFFAFAD2);

    /// <summary>
    /// LightGray color.
    /// </summary>
    public static readonly Color LightGray = FromBGRA(0xFFD3D3D3);

    /// <summary>
    /// LightGreen color.
    /// </summary>
    public static readonly Color LightGreen = FromBGRA(0xFF90EE90);

    /// <summary>
    /// LightPink color.
    /// </summary>
    public static readonly Color LightPink = FromBGRA(0xFFFFB6C1);

    /// <summary>
    /// LightSalmon color.
    /// </summary>
    public static readonly Color LightSalmon = FromBGRA(0xFFFFA07A);

    /// <summary>
    /// LightSeaGreen color.
    /// </summary>
    public static readonly Color LightSeaGreen = FromBGRA(0xFF20B2AA);

    /// <summary>
    /// LightSkyBlue color.
    /// </summary>
    public static readonly Color LightSkyBlue = FromBGRA(0xFF87CEFA);

    /// <summary>
    /// LightSlateGray color.
    /// </summary>
    public static readonly Color LightSlateGray = FromBGRA(0xFF778899);

    /// <summary>
    /// LightSteelBlue color.
    /// </summary>
    public static readonly Color LightSteelBlue = FromBGRA(0xFFB0C4DE);

    /// <summary>
    /// LightYellow color.
    /// </summary>
    public static readonly Color LightYellow = FromBGRA(0xFFFFFFE0);

    /// <summary>
    /// Lime color.
    /// </summary>
    public static readonly Color Lime = FromBGRA(0xFF00FF00);

    /// <summary>
    /// LimeGreen color.
    /// </summary>
    public static readonly Color LimeGreen = FromBGRA(0xFF32CD32);

    /// <summary>
    /// Linen color.
    /// </summary>
    public static readonly Color Linen = FromBGRA(0xFFFAF0E6);

    /// <summary>
    /// Magenta color.
    /// </summary>
    public static readonly Color Magenta = FromBGRA(0xFFFF00FF);

    /// <summary>
    /// Maroon color.
    /// </summary>
    public static readonly Color Maroon = FromBGRA(0xFF800000);

    /// <summary>
    /// MediumAquamarine color.
    /// </summary>
    public static readonly Color MediumAquamarine = FromBGRA(0xFF66CDAA);

    /// <summary>
    /// MediumBlue color.
    /// </summary>
    public static readonly Color MediumBlue = FromBGRA(0xFF0000CD);

    /// <summary>
    /// MediumOrchid color.
    /// </summary>
    public static readonly Color MediumOrchid = FromBGRA(0xFFBA55D3);

    /// <summary>
    /// MediumPurple color.
    /// </summary>
    public static readonly Color MediumPurple = FromBGRA(0xFF9370DB);

    /// <summary>
    /// MediumSeaGreen color.
    /// </summary>
    public static readonly Color MediumSeaGreen = FromBGRA(0xFF3CB371);

    /// <summary>
    /// MediumSlateBlue color.
    /// </summary>
    public static readonly Color MediumSlateBlue = FromBGRA(0xFF7B68EE);

    /// <summary>
    /// MediumSpringGreen color.
    /// </summary>
    public static readonly Color MediumSpringGreen = FromBGRA(0xFF00FA9A);

    /// <summary>
    /// MediumTurquoise color.
    /// </summary>
    public static readonly Color MediumTurquoise = FromBGRA(0xFF48D1CC);

    /// <summary>
    /// MediumVioletRed color.
    /// </summary>
    public static readonly Color MediumVioletRed = FromBGRA(0xFFC71585);

    /// <summary>
    /// MidnightBlue color.
    /// </summary>
    public static readonly Color MidnightBlue = FromBGRA(0xFF191970);

    /// <summary>
    /// MintCream color.
    /// </summary>
    public static readonly Color MintCream = FromBGRA(0xFFF5FFFA);

    /// <summary>
    /// MistyRose color.
    /// </summary>
    public static readonly Color MistyRose = FromBGRA(0xFFFFE4E1);

    /// <summary>
    /// Moccasin color.
    /// </summary>
    public static readonly Color Moccasin = FromBGRA(0xFFFFE4B5);

    /// <summary>
    /// NavajoWhite color.
    /// </summary>
    public static readonly Color NavajoWhite = FromBGRA(0xFFFFDEAD);

    /// <summary>
    /// Navy color.
    /// </summary>
    public static readonly Color Navy = FromBGRA(0xFF000080);

    /// <summary>
    /// OldLace color.
    /// </summary>
    public static readonly Color OldLace = FromBGRA(0xFFFDF5E6);

    /// <summary>
    /// Olive color.
    /// </summary>
    public static readonly Color Olive = FromBGRA(0xFF808000);

    /// <summary>
    /// OliveDrab color.
    /// </summary>
    public static readonly Color OliveDrab = FromBGRA(0xFF6B8E23);

    /// <summary>
    /// Orange color.
    /// </summary>
    public static readonly Color Orange = FromBGRA(0xFFFFA500);

    /// <summary>
    /// OrangeRed color.
    /// </summary>
    public static readonly Color OrangeRed = FromBGRA(0xFFFF4500);

    /// <summary>
    /// Orchid color.
    /// </summary>
    public static readonly Color Orchid = FromBGRA(0xFFDA70D6);

    /// <summary>
    /// PaleGoldenrod color.
    /// </summary>
    public static readonly Color PaleGoldenrod = FromBGRA(0xFFEEE8AA);

    /// <summary>
    /// PaleGreen color.
    /// </summary>
    public static readonly Color PaleGreen = FromBGRA(0xFF98FB98);

    /// <summary>
    /// PaleTurquoise color.
    /// </summary>
    public static readonly Color PaleTurquoise = FromBGRA(0xFFAFEEEE);

    /// <summary>
    /// PaleVioletRed color.
    /// </summary>
    public static readonly Color PaleVioletRed = FromBGRA(0xFFDB7093);

    /// <summary>
    /// PapayaWhip color.
    /// </summary>
    public static readonly Color PapayaWhip = FromBGRA(0xFFFFEFD5);

    /// <summary>
    /// PeachPuff color.
    /// </summary>
    public static readonly Color PeachPuff = FromBGRA(0xFFFFDAB9);

    /// <summary>
    /// Peru color.
    /// </summary>
    public static readonly Color Peru = FromBGRA(0xFFCD853F);

    /// <summary>
    /// Pink color.
    /// </summary>
    public static readonly Color Pink = FromBGRA(0xFFFFC0CB);

    /// <summary>
    /// Plum color.
    /// </summary>
    public static readonly Color Plum = FromBGRA(0xFFDDA0DD);

    /// <summary>
    /// PowderBlue color.
    /// </summary>
    public static readonly Color PowderBlue = FromBGRA(0xFFB0E0E6);

    /// <summary>
    /// Purple color.
    /// </summary>
    public static readonly Color Purple = FromBGRA(0xFF800080);

    /// <summary>
    /// Red color.
    /// </summary>
    public static readonly Color Red = FromBGRA(0xFFFF0000);

    /// <summary>
    /// RosyBrown color.
    /// </summary>
    public static readonly Color RosyBrown = FromBGRA(0xFFBC8F8F);

    /// <summary>
    /// RoyalBlue color.
    /// </summary>
    public static readonly Color RoyalBlue = FromBGRA(0xFF4169E1);

    /// <summary>
    /// SaddleBrown color.
    /// </summary>
    public static readonly Color SaddleBrown = FromBGRA(0xFF8B4513);

    /// <summary>
    /// Salmon color.
    /// </summary>
    public static readonly Color Salmon = FromBGRA(0xFFFA8072);

    /// <summary>
    /// SandyBrown color.
    /// </summary>
    public static readonly Color SandyBrown = FromBGRA(0xFFF4A460);

    /// <summary>
    /// SeaGreen color.
    /// </summary>
    public static readonly Color SeaGreen = FromBGRA(0xFF2E8B57);

    /// <summary>
    /// SeaShell color.
    /// </summary>
    public static readonly Color SeaShell = FromBGRA(0xFFFFF5EE);

    /// <summary>
    /// Sienna color.
    /// </summary>
    public static readonly Color Sienna = FromBGRA(0xFFA0522D);

    /// <summary>
    /// Silver color.
    /// </summary>
    public static readonly Color Silver = FromBGRA(0xFFC0C0C0);

    /// <summary>
    /// SkyBlue color.
    /// </summary>
    public static readonly Color SkyBlue = FromBGRA(0xFF87CEEB);

    /// <summary>
    /// SlateBlue color.
    /// </summary>
    public static readonly Color SlateBlue = FromBGRA(0xFF6A5ACD);

    /// <summary>
    /// SlateGray color.
    /// </summary>
    public static readonly Color SlateGray = FromBGRA(0xFF708090);

    /// <summary>
    /// Snow color.
    /// </summary>
    public static readonly Color Snow = FromBGRA(0xFFFFFAFA);

    /// <summary>
    /// SpringGreen color.
    /// </summary>
    public static readonly Color SpringGreen = FromBGRA(0xFF00FF7F);

    /// <summary>
    /// SteelBlue color.
    /// </summary>
    public static readonly Color SteelBlue = FromBGRA(0xFF4682B4);

    /// <summary>
    /// Tan color.
    /// </summary>
    public static readonly Color Tan = FromBGRA(0xFFD2B48C);

    /// <summary>
    /// Teal color.
    /// </summary>
    public static readonly Color Teal = FromBGRA(0xFF008080);

    /// <summary>
    /// Thistle color.
    /// </summary>
    public static readonly Color Thistle = FromBGRA(0xFFD8BFD8);

    /// <summary>
    /// Tomato color.
    /// </summary>
    public static readonly Color Tomato = FromBGRA(0xFFFF6347);

    /// <summary>
    /// Turquoise color.
    /// </summary>
    public static readonly Color Turquoise = FromBGRA(0xFF40E0D0);

    /// <summary>
    /// Violet color.
    /// </summary>
    public static readonly Color Violet = FromBGRA(0xFFEE82EE);

    /// <summary>
    /// Wheat color.
    /// </summary>
    public static readonly Color Wheat = FromBGRA(0xFFF5DEB3);

    /// <summary>
    /// White color.
    /// </summary>
    public static readonly Color White = FromBGRA(0xFFFFFFFF);

    /// <summary>
    /// WhiteSmoke color.
    /// </summary>
    public static readonly Color WhiteSmoke = FromBGRA(0xFFF5F5F5);

    /// <summary>
    /// Yellow color.
    /// </summary>
    public static readonly Color Yellow = FromBGRA(0xFFFFFF00);

    /// <summary>
    /// YellowGreen color.
    /// </summary>
    public static readonly Color YellowGreen = FromBGRA(0xFF9ACD32);
}
