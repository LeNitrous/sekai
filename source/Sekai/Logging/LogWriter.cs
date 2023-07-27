// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

/// <summary>
/// Writes <see cref="LogMessage"/>s to output sources.
/// </summary>
public abstract class LogWriter : IDisposable
{
    /// <summary>
    /// The number of times this writer has logged.
    /// </summary>
    public int LogCount { get; internal set; }

    public abstract void Dispose();

    /// <summary>
    /// Flushes the contents of this <see cref="LogWriter"/>.
    /// </summary>
    public abstract void Flush();

    /// <summary>
    /// Writes a <see cref="LogMessage"/> to this writer.
    /// </summary>
    /// <param name="message"></param>
    public abstract void Write(LogMessage message);
}
