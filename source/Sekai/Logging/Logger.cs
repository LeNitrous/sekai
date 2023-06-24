// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sekai.Logging;

/// <summary>
/// Logs messages to output channels.
/// </summary>
public sealed class Logger
{
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

    private readonly string name;
    private readonly object? sync;
    private readonly Logger? owner;
    private readonly List<LogWriter>? writers;
    private readonly Dictionary<string, Logger>? loggers;

    public Logger()
    {
        name = string.Empty;
        sync = new();
        writers = new();
        loggers = new() { { name, this } };
    }

    private Logger(Logger owner, string name)
    {
        this.name = name;
        this.owner = owner;
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(object? message)
    {
        broadcast(new(name, message, LogLevel.Verbose));
    }

    /// <summary>
    /// Logs a formatted informational message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Log([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Log(message);
        }
        else
        {
            Log((object)string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Trace(object? message)
    {
        broadcast(new(name, message, LogLevel.Trace));
    }

    /// <summary>
    /// Logs a formatted trace message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Trace(message);
        }
        else
        {
            Trace((object)string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(object? message)
    {
        broadcast(new(name, message, LogLevel.Debug));
    }

    /// <summary>
    /// Logs a formatted debug message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Debug(message);
        }
        else
        {
            Debug((object)string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Warn(object? message, Exception? exception = null)
    {
        broadcast(new(name, message, LogLevel.Warning, exception: exception));
    }

    /// <summary>
    /// Logs a formatted warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Warn(message, exception);
        }
        else
        {
            Warn((object)string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Error(object? message, Exception? exception = null)
    {
        broadcast(new(name, message, LogLevel.Error, exception: exception));
    }

    /// <summary>
    /// Logs a formatted error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Error(message, exception);
        }
        else
        {
            Error((object)string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Critical(object? message, Exception? exception = null)
    {
        broadcast(new(name, message, LogLevel.Critical, exception: exception));
    }

    /// <summary>
    /// Logs a formatted critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    public void Critical([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Critical(message, exception);
        }
        else
        {
            Critical((object)string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }

    /// <summary>
    /// Creates a named logger.
    /// </summary>
    /// <param name="name">The logger's name.</param>
    /// <returns>A new named logger.</returns>
    public Logger Create(string name)
    {
        return create(name);
    }

    /// <summary>
    /// Adds an output source.
    /// </summary>
    /// <param name="writer">The writer to add.</param>
    public void AddOutput(LogWriter writer)
    {
        if (owner is not null)
        {
            owner.AddOutput(writer);
        }
        else if (writers is not null)
        {
            writers.Add(writer);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Removes an output source.
    /// </summary>
    /// <param name="writer">The writer to remove.</param>
    /// <returns>Returns <see langword="true"/> if the writer has been removed. Otherwise, returns <see langword="false"/>.</returns>
    public bool RemoveOutput(LogWriter writer)
    {
        if (owner is not null)
        {
            return owner.RemoveOutput(writer);
        }
        else if (writers is not null)
        {
            return writers.Remove(writer);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private Logger create(string name)
    {
        if (loggers is not null)
        {
            if (!loggers.TryGetValue(name, out var logger))
            {
                logger = new Logger(this, name)
                {
                    MinimumLevel = MinimumLevel,
                    MaximumLevel = MaximumLevel,
                };

                loggers.Add(name, logger);
            }

            return logger;
        }
        else if (owner is not null)
        {
            return owner.create(name);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private void broadcast(LogMessage message)
    {
        if (owner is null)
        {
            broadcast(message, MinimumLevel, MaximumLevel, Filter);
        }
        else
        {
            owner.broadcast(message, MinimumLevel, MaximumLevel, Filter ?? owner.Filter);
        }
    }

    private void broadcast(LogMessage message, LogLevel minimum, LogLevel maximum, Func<LogMessage, bool>? filter)
    {
        if (writers is null || sync is null)
        {
            return;
        }

        if (message.Level > maximum || message.Level < minimum)
        {
            return;
        }

        if (filter is not null && filter(message))
        {
            return;
        }

        lock (sync)
        {
            foreach (var writer in writers)
            {
                if (message.Level > writer.MaximumLevel || message.Level < writer.MinimumLevel)
                {
                    continue;
                }

                if (writer.Filter is not null && writer.Filter(message))
                {
                    continue;
                }

                writer.Write(message);
                writer.LogCount++;

                if ((writer.LogCount % writer.Frequency) != 0)
                {
                    continue;
                }

                writer.Flush();
            }
        }
    }
}
