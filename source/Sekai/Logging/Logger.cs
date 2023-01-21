// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

public sealed class Logger : FrameworkObject
{
    /// <summary>
    /// The logger's name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Get or set whether logging is enabled or not.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Called when a new message is being logged.
    /// </summary>
    public event Action<LogMessage>? OnMessageLogged;

    internal Logger(string? name = null)
    {
        Name = name;
    }

    /// <summary>
    /// Logs a message at a given level.
    /// </summary>
    public void Log(object? message, LogLevel level = LogLevel.Verbose) => log(new LogMessage(message, Name, level));

    /// <summary>
    /// Logs an information level message.
    /// </summary>
    public void Info(object? message) => log(new LogMessage(message, Name, LogLevel.Information));

    /// <summary>
    /// Logs a debug level message.
    /// </summary>
    public void Debug(object? message) => log(new LogMessage(message, Name, LogLevel.Debug));

    /// <summary>
    /// Logs an error level message.
    /// </summary>
    public void Error(object? message, Exception? exception = null) => log(new LogMessage(message, Name, LogLevel.Error, exception));

    /// <summary>
    /// Logs a verbose level message.
    /// </summary>
    public void Verbose(object? message) => log(new LogMessage(message, Name, LogLevel.Verbose));

    /// <summary>
    /// Logs a warning level message.
    /// </summary>
    public void Warning(object? message) => log(new LogMessage(message, Name, LogLevel.Warning));

    private void log(LogMessage message)
    {
        if (!Enabled)
            return;

        OnMessageLogged?.Invoke(message);
    }
}
