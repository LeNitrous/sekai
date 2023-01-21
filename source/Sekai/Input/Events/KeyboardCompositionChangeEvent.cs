// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a keyboard composition change event.
/// </summary>
public class KeyboardCompositionChangeEvent : KeyboardEvent
{
    /// <summary>
    /// The position of the cursor.
    /// </summary>
    public readonly int Position;

    /// <summary>
    /// The length of the text.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// The composition text.
    /// </summary>
    public readonly string Text;

    public KeyboardCompositionChangeEvent(int position, int length, string text)
    {
        Position = position;
        Length = length;
        Text = text;
    }
}
