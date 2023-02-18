// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogSink"/> component that formats a <see cref="LogMessage"/> before being written.
/// </summary>
public abstract class LogFormatter
{
    /// <summary>
    /// The default log formatter.
    /// </summary>
    public static LogFormatter Default { get; } = new LogFormatterDefault();

    /// <summary>
    /// Formats the given message log.
    /// </summary>
    /// <param name="message">The message log to be formatted.</param>
    /// <returns>The formatted message log.</returns>
    public abstract string Format(LogMessage message);
}
