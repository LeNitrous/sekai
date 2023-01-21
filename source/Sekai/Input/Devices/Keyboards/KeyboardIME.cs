// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.Keyboards;

/// <summary>
/// A keyboard IME (input method editor) event data.
/// </summary>
public readonly struct KeyboardIME : IEquatable<KeyboardIME>
{
    /// <summary>
    /// The null-terminated text in UTF8 encoding.
    /// </summary>
    public readonly string Text;

    /// <summary>
    /// The index where to begin text editing.
    /// </summary>
    public readonly int Start;

    /// <summary>
    /// The number of characters to edit from the start point.
    /// </summary>
    public readonly int Length;

    public KeyboardIME(string text, int start, int length)
    {
        Text = text;
        Start = start;
        Length = length;
    }

    public override bool Equals(object? obj)
    {
        return obj is KeyboardIME ime && Equals(ime);
    }

    public bool Equals(KeyboardIME other)
    {
        return Text == other.Text && Start == other.Start && Length == other.Length;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text, Start, Length);
    }

    public static bool operator ==(KeyboardIME left, KeyboardIME right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(KeyboardIME left, KeyboardIME right)
    {
        return !(left == right);
    }
}
