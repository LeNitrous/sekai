// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Logging;

public sealed partial class Logger : ILogger
{
    public string? Name { get; }
    public bool Enabled { get; set; } = true;

    private event Action<LogMessage>? onMessageLogged;
    event Action<LogMessage> ILogger.OnMessageLogged
    {
        add => onMessageLogged += value;
        remove => onMessageLogged -= value;
    }

    private Logger(string? name = null)
    {
        Name = name;
    }

    void ILogger.Debug(string message)
    {
        log(new LogMessage(message, Name, LogLevel.Debug));
    }

    void ILogger.Error(string message, Exception? exception)
    {
        log(new LogMessage(message, Name, LogLevel.Error, exception));
    }

    void ILogger.Info(string message)
    {
        log(new LogMessage(message, Name, LogLevel.Information));
    }

    void ILogger.Log(string message, LogLevel level)
    {
        log(new LogMessage(message, Name, level));
    }

    void ILogger.Verbose(string message)
    {
        log(new LogMessage(message, Name, LogLevel.Verbose));
    }

    void ILogger.Warning(string message)
    {
        log(new LogMessage(message, Name, LogLevel.Warning));
    }

    private void log(LogMessage message)
    {
        if (!Enabled)
            return;

        onMessageLogged?.Invoke(message);
        OnMessageLogged?.Invoke(message);
    }
}

public static class LoggerExtensions
{
    public static void Log(this Logger logger, string message, LogLevel level = LogLevel.Verbose)
    {
        ((ILogger)logger).Log(message, level);
    }

    public static void Debug(this Logger logger, string message)
    {
        ((ILogger)logger).Debug(message);
    }

    public static void Error(this Logger logger, string message, Exception? exception = null)
    {
        ((ILogger)logger).Error(message, exception);
    }

    public static void Info(this Logger logger, string message)
    {
        ((ILogger)logger).Info(message);
    }

    public static void Verbose(this Logger logger, string message)
    {
        ((ILogger)logger).Verbose(message);
    }

    public static void Warning(this Logger logger, string message)
    {
        ((ILogger)logger).Warning(message);
    }
}
