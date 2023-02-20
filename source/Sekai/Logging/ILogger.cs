// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

/// <summary>
/// Represents a logger.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs a message at a verbose level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments passed in formatting the message.</param>
    void Log(object? message, params object[] args);

    /// <summary>
    /// Logs an information level message.
    /// </summary>
    /// <inheritdoc cref="Log(object?, object[])"/>
    void Info(object? message, params object[] args);

    /// <summary>
    /// Logs a warning level message.
    /// </summary>
    /// <inheritdoc cref="Log(object?, object[])"/>
    void Warn(object? message, params object[] args);

    /// <summary>
    /// Logs a debug level message.
    /// </summary>
    /// <inheritdoc cref="Log(object?, object[])"/>
    void Debug(object? message, params object[] args);

    /// <summary>
    /// Logs an error level message.
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    /// <inheritdoc cref="Log(object?, object[])"/>
    void Error(object? message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Gets the logger for the given category while inheriting the previous logger's categories.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <returns>A new logger with the specified category.</returns>
    ILogger GetLogger(string category);
}
