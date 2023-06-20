// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Audio;

/// <summary>
/// An enumeration of Audio APIs.
/// </summary>
public enum AudioAPI
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// OpenAL.
    /// </summary>
    OpenAL,

    /// <summary>
    /// Dummy.
    /// </summary>
    Dummy = int.MaxValue,
}
