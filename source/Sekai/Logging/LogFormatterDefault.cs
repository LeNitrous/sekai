// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Text;

namespace Sekai.Logging;

/// <summary>
/// The default implementation of a <see cref="LogFormatter"/>.
/// </summary>
public class LogFormatterDefault : LogFormatter
{
    /// <summary>
    /// Formats a timestamp.
    /// </summary>
    /// <param name="t">The timestamp.</param>
    /// <returns>The stringified timestamp.</returns>
    protected virtual string FormatTimestamp(LogMessage m) => m.Timestamp.ToString("dd/MM/yyyy hh:mm:ss tt");

    /// <summary>
    /// Formats an exception.
    /// </summary>
    /// <param name="e">The exception.</param>
    /// <returns>The stringified exception.</returns>
    protected virtual string FormatException(LogMessage m) => m.Exception?.ToString() ?? string.Empty;

    /// <summary>
    /// Formats a log level.
    /// </summary>
    /// <param name="l">The log level.</param>
    /// <returns>The stringified log level.</returns>
    protected virtual string FormatLogLevel(LogMessage m)
    {
        switch (m.Level)
        {
            default:
            case LogLevel.Verbose:
                return "   ";

            case LogLevel.Debug:
                return "DBG";

            case LogLevel.Information:
                return "INF";

            case LogLevel.Warning:
                return "WRN";

            case LogLevel.Error:
                return "ERR";
        }
    }

    /// <summary>
    /// Formats a message.
    /// </summary>
    /// <param name="m">The message.</param>
    /// <param name="args">The arguments passed.</param>
    /// <returns>The stringified message.</returns>
    protected virtual string FormatMessage(LogMessage m)
    {
        if (m.Message is string s)
        {
            return string.Format(s, m.Arguments);
        }
        else
        {
            return m.Message?.ToString() ?? "null";
        }
    }

    /// <summary>
    /// Formats tags.
    /// </summary>
    /// <param name="tags">The tags.</param>
    /// <returns>The stringified tags.</returns>
    protected virtual string FormatCategory(LogMessage m)
    {
        return string.Join('.', m.Tags);
    }

    public override string Format(LogMessage message)
    {
        var builder = new StringBuilder();
        builder.Append(FormatTimestamp(message));
        builder.Append(' ');
        builder.Append(FormatLogLevel(message));
        builder.Append(' ');

        if (message.Tags.Length > 0)
        {
            builder.Append(FormatCategory(message));
            builder.Append(' ');
        }

        builder.Append(FormatMessage(message));

        if (message.Exception is not null)
        {
            builder.AppendLine();
            builder.Append(FormatException(message));
        }

        return builder.ToString();
    }
}
