// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Keyboards;

namespace Sekai.Input.States;

public sealed class KeyboardState : InputState<Key>
{
    public int Position { get; set; }
    public int Length { get; set; }
    public string Text { get; set; } = string.Empty;
    public string TextPending { get; set; } = string.Empty;

    public override void Reset()
    {
        base.Reset();
        Position = 0;
        Length = 0;
        Text = string.Empty;
        TextPending = string.Empty;
    }
}
