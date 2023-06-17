// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sekai.Framework.Logging;

/// <summary>
/// Information about a log entry.
/// </summary>
public readonly struct LogMessage : IEquatable<LogMessage>
{
    /// <summary>
    /// The category of this message.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// The logged message.
    /// </summary>
    public object? Content { get; }

    /// <summary>
    /// The log level of this message.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// The timestamp of this logged message.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// The exception associated with this logged message.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Creates a new <see cref="LogMessage"/>
    /// </summary>
    /// <param name="category">The log entry's category.</param>
    /// <param name="message">The log entry's message.</param>
    /// <param name="level">The log entry's level.</param>
    /// <param name="timestamp">The log entry's timestamp.</param>
    /// <param name="exception">The exception associated with this entry.</param>
    public LogMessage(string category, object? message = null, LogLevel level = LogLevel.Trace, DateTime? timestamp = null, Exception? exception = null)
    {
        Level = level;
        Content = message;
        Category = category;
        Timestamp = timestamp ?? DateTime.Now;
        Exception = exception;
    }

    public bool Equals(LogMessage other)
    {
        return Timestamp.Equals(other.Timestamp)
            && EqualityComparer<object>.Default.Equals(Content, other.Content)
            && EqualityComparer<LogLevel>.Default.Equals(Level, other.Level)
            && EqualityComparer<Exception?>.Default.Equals(Exception, other.Exception);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is LogMessage message && Equals(message);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Content, Level, Timestamp, Exception);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Content is not null)
        {
            builder.AppendLine(Content.ToString());
        }

        if (Exception is not null)
        {
            builder.AppendLine(Exception.ToString());
        }

        return builder.ToString();
    }

    public static bool operator ==(LogMessage left, LogMessage right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LogMessage left, LogMessage right)
    {
        return !(left == right);
    }
}
