// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sekai.Logging;

/// <summary>
/// Logs messages to output channels.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// The logger's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(object? message);

    /// <summary>
    /// Logs a formatted informational message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Log([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Log(message);
        }
        else
        {
            Log(string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Trace(object? message);

    /// <summary>
    /// Logs a formatted trace message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Trace(message);
        }
        else
        {
            Trace(string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(object? message);

    /// <summary>
    /// Logs a formatted debug message.
    /// </summary>
    /// <param name="message">The formatted message to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[]? args)
    {
        if (args is null)
        {
            Debug(message);
        }
        else
        {
            Debug(string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Warn(object? message, Exception? exception = null);

    /// <summary>
    /// Logs a formatted warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Warn(message, exception);
        }
        else
        {
            Warn(string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Error(object? message, Exception? exception = null);

    /// <summary>
    /// Logs a formatted error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Error(message, exception);
        }
        else
        {
            Error(string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Critical(object? message, Exception? exception = null);

    /// <summary>
    /// Logs a formatted critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="args">The arguments to include in formatting.</param>
    void Critical([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, Exception? exception = null, params object?[]? args)
    {
        if (args is null)
        {
            Critical(message, exception);
        }
        else
        {
            Critical(string.Format(CultureInfo.CurrentCulture, message, args), exception);
        }
    }
}
