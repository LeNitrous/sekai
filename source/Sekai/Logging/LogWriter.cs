// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

/// <summary>
/// Writes <see cref="LogMessage"/>s to output sources.
/// </summary>
public abstract class LogWriter
{
    /// <summary>
    /// The frequency of logging.
    /// </summary>
    public int Frequency = 1;

    /// <summary>
    /// The minimum log level.
    /// </summary>
    public LogLevel MinimumLevel = LogLevel.Trace;

    /// <summary>
    /// The maximum log level.
    /// </summary>
    public LogLevel MaximumLevel = LogLevel.Critical;

    /// <summary>
    /// The action used to filter messages.
    /// </summary>
    /// <remarks>
    /// Return <see langword="true"/> to filter the message. Otherwise, return <see langword="false"/>
    /// </remarks>
    public Func<LogMessage, bool>? Filter;

    /// <summary>
    /// The number of times this writer has logged.
    /// </summary>
    internal int LogCount;

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
