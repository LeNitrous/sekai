// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a keyboard composition submission event.
/// </summary>
public class KeyboardCompositionSubmitEvent : KeyboardEvent
{
    /// <summary>
    /// The submitted text.
    /// </summary>
    public readonly string Text;

    public KeyboardCompositionSubmitEvent(string text)
    {
        Text = text;
    }
}
