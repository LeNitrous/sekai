// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.IO.HID;
/// <summary>
/// A struct defining a Key button command.
/// </summary>
public struct ButtonCommand
{
    /// <summary>
    /// The name of the Key button command.
    /// Ex: IN_STRAFE.
    /// </summary>
    public string Name;

    /// <summary>
    /// The keys holding it down.
    /// </summary>
    public KeyName[]? KeysDown;

    /// <summary>
    /// The current state of the key button command.
    /// </summary>
    public bool Active;
}
