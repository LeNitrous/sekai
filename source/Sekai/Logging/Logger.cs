// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Platform;

namespace Sekai.Logging;

/// <inheritdoc cref="ILogger"/>
public static class Logger
{
    /// <summary>
    /// The minimum log level.
    /// </summary>
    public static LogLevel MinimumLevel = RuntimeInfo.IsDebug ? LogLevel.Trace : LogLevel.Debug;

    /// <summary>
    /// The maximum log level.
    /// </summary>
    public static LogLevel MaximumLevel = LogLevel.Critical;

    /// <summary>
    /// The frequency of logging.
    /// </summary>
    public static int Frequency;

    /// <summary>
    /// The action used to filter messages.
    /// </summary>
    /// <remarks>
    /// Return <see langword="true"/> to filter the message. Otherwise, return <see langword="false"/>
    /// </remarks>
    public static Func<LogMessage, bool>? Filter;

    private static readonly ILogger instance;
    private static readonly List<LogWriter> writers = new();
    private static readonly Dictionary<string, ILogger> loggers = new();

    static Logger()
    {
        instance = Create(string.Empty);
    }

    /// <summary>
    /// Creates a named logger.
    /// </summary>
    /// <param name="name">The logger's name.</param>
    /// <returns>A new named logger.</returns>
    public static ILogger Create(string name)
    {
        return create(name);
    }

    /// <summary>
    /// Creates a named logger.
    /// </summary>
    /// <typeparam name="T">The target type of the logger.</typeparam>
    /// <returns>A new named logger.</returns>
    public static ILogger Create<T>()
    {
        return create(typeof(T).Name);
    }

    /// <summary>
    /// Adds a writer to this logger.
    /// </summary>
    /// <param name="writer">The writer to add.</param>
    public static void AddWriter(LogWriter writer)
    {
        writers.Add(writer);
    }

    /// <summary>
    /// Removes a writer from this logger.
    /// </summary>
    /// <param name="writer">The writer to remove.</param>
    /// <returns>Returns <see langword="true"/> if the writer has been removed. Otherwise, returns <see langword="false"/>.</returns>
    public static bool RemoveWriter(LogWriter writer)
    {
        return writers.Remove(writer);
    }

    /// <inheritdoc cref="ILogger.Trace(object?)"/>
    public static void Trace(object? message) => instance.Trace(message);

    /// <inheritdoc cref="ILogger.Debug(object?)"/>
    public static void Debug(object? message) => instance.Debug(message);

    /// <inheritdoc cref="ILogger.Log(object?)"/>
    public static void Log(object? message) => instance.Log(message);

    /// <inheritdoc cref="ILogger.Warn(object?, Exception?)"/>
    public static void Warn(object? message, Exception? exception = null) => instance.Warn(message, exception);

    /// <inheritdoc cref="ILogger.Error(object?, Exception?)"/>
    public static void Error(object? message, Exception? exception = null) => instance.Error(message, exception);

    /// <inheritdoc cref="ILogger.Critical(object?, Exception?)"/>
    public static void Critical(object? message, Exception? exception = null) => instance.Critical(message, exception);

    private static ILogger create(string name)
    {
        if (!loggers.TryGetValue(name, out var logger))
        {
            logger = new NamedLogger(name);
            loggers.Add(name, logger);
        }

        return logger;
    }

    internal static void Broadcast(LogMessage message)
    {
        if (message.Level > MinimumLevel || message.Level < MinimumLevel)
        {
            return;
        }

        if (Filter is not null && Filter(message))
        {
            return;
        }

        foreach (var writer in writers)
        {
            writer.Receive(message);
        }
    }
}

internal class NamedLogger : ILogger
{
    public string Name { get; }

    public NamedLogger(string name)
    {
        Name = name;
    }

    public void Critical(object? message, Exception? exception = null) => Logger.Broadcast(new(Name, message, LogLevel.Critical, exception: exception));

    public void Debug(object? message) => Logger.Broadcast(new(Name, message, LogLevel.Debug));

    public void Error(object? message, Exception? exception = null) => Logger.Broadcast(new(Name, message, LogLevel.Error, exception: exception));

    public void Log(object? message) => Logger.Broadcast(new(Name, message, LogLevel.Information));

    public void Trace(object? message) => Logger.Broadcast(new(Name, message, LogLevel.Trace));

    public void Warn(object? message, Exception? exception = null) => Logger.Broadcast(new(Name, message, LogLevel.Warning, exception: exception));
}
