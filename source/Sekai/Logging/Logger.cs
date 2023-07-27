// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sekai.Logging;

/// <inheritdoc cref="ILogger"/>
internal sealed class Logger : ILogger, IDisposable
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
    /// The predicate used to filter messages.
    /// </summary>
    /// <remarks>
    /// Return <see langword="true"/> to filter the message. Otherwise, return <see langword="false"/>
    /// </remarks>
    public Predicate<LogMessage>? Filter;

    private bool isDisposed;
    private readonly int frequency;
    private readonly LogLevel minimum;
    private readonly LogLevel maximum;
    private readonly Predicate<LogMessage>? filter;
    private readonly IReadOnlyList<LogWriter> writers;

    public Logger(IReadOnlyList<LogWriter> writers, int frequency, LogLevel minimum, LogLevel maximum, Predicate<LogMessage>? filter)
    {
        this.filter = filter;
        this.minimum = minimum;
        this.maximum = maximum;
        this.writers = writers;
        this.frequency = frequency;
    }

    public void Log(object? message)
    {
        broadcast(new LogMessage(message, LogLevel.Verbose));
    }

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

    public void Trace(object? message)
    {
        broadcast(new LogMessage(message, LogLevel.Trace));
    }

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

    public void Debug(object? message)
    {
        broadcast(new LogMessage(message, LogLevel.Debug));
    }

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

    public void Warn(object? message, Exception? exception)
    {
        broadcast(new LogMessage(message, LogLevel.Warning, exception: exception));
    }

    public void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception, params object?[]? args)
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

    public void Error(object? message, Exception? exception)
    {
        broadcast(new LogMessage(message, LogLevel.Error, exception: exception));
    }

    public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception, params object?[]? args)
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

    public void Critical(object? message, Exception? exception)
    {
        broadcast(new LogMessage(message, LogLevel.Critical, exception: exception));
    }

    public void Critical([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception, params object?[]? args)
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

    private void broadcast(LogMessage message)
    {
        if (message.Level > maximum || message.Level < minimum)
        {
            return;
        }

        if (filter is not null && filter(message))
        {
            return;
        }

        foreach (var writer in writers)
        {
            writer.Write(message);

            if ((writer.LogCount % frequency) != 0)
            {
                continue;
            }

            writer.Flush();

            writer.LogCount++;
        }
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        foreach (var writer in writers)
        {
            writer.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
