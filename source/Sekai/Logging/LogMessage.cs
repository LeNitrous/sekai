// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Logging;

/// <summary>
/// Represents a logged message.
/// </summary>
public readonly struct LogMessage : IEquatable<LogMessage>
{
    /// <summary>
    /// The template message logged.
    /// </summary>
    public readonly object? Message;

    /// <summary>
    /// The arguments passed.
    /// </summary>
    public readonly object?[] Arguments;

    /// <summary>
    /// The level this message was logged under.
    /// </summary>
    public readonly LogLevel Level;

    /// <summary>
    /// The message timestamp.
    /// </summary>
    public readonly DateTime Timestamp;

    /// <summary>
    /// The exception containing this message.
    /// </summary>
    public readonly Exception? Exception;

    /// <summary>
    /// The log message's tags.
    /// </summary>
    public readonly string[] Tags;

    public LogMessage(object? message, object?[]? arguments = null, LogLevel level = LogLevel.Verbose, string[]? tags = null, Exception? exception = null)
    {
        Tags = tags ?? Array.Empty<string>();
        Message = message;
        Arguments = arguments ?? Array.Empty<object>();
        Level = level;
        Timestamp = DateTime.Now;
        Exception = exception;
    }

    public override bool Equals(object? obj)
    {
        return obj is LogMessage message && Equals(message);
    }

    public bool Equals(LogMessage other)
    {
        return EqualityComparer<object?>.Default.Equals(Message, other.Message) &&
               EqualityComparer<object?[]>.Default.Equals(Arguments, other.Arguments) &&
               Level == other.Level &&
               Timestamp == other.Timestamp &&
               EqualityComparer<Exception?>.Default.Equals(Exception, other.Exception);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Message, Arguments, Level, Timestamp, Exception);
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

