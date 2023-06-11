// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Logging;

/// <summary>
/// The level of the log entry.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Used for more granular debug messages.
    /// </summary>
    Trace,

    /// <summary>
    /// Used for debug messages.
    /// </summary>
    Debug,

    /// <summary>
    /// Used for general messages.
    /// </summary>
    Information,

    /// <summary>
    /// Used for informing unexpected behavior but the application can still be operational.
    /// </summary>
    Warning,

    /// <summary>
    /// Used for informing unexpected behavior that prevents some functionality in the application.
    /// </summary>
    Error,

    /// <summary>
    /// Used for informing unexpected behavior that prevents the application from further operation.
    /// </summary>
    Critical
}
