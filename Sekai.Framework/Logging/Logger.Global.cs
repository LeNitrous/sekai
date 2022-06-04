// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Logging;

public partial class Logger
{
    public static event Action<LogMessage>? OnMessageLogged;
    private static readonly ILogger global = new Logger();
    private static readonly Dictionary<string, ILogger> loggers = new();

    public static void Debug(string message)
    {
        global.Debug(message);
    }

    public static void Error(string message, Exception? exception = null)
    {
        global.Error(message, exception);
    }

    public static void Info(string message)
    {
        global.Info(message);
    }

    public static void Log(string message, LogLevel level = LogLevel.Verbose)
    {
        global.Log(message, level);
    }

    public static void Verbose(string message)
    {
        global.Verbose(message);
    }

    public static void Warning(string message)
    {
        global.Warning(message);
    }

    public static ILogger GetLogger(string name)
    {
        if (!loggers.TryGetValue(name, out var logger))
        {
            logger = new Logger(name);
            loggers.Add(name, logger);
        }

        return logger;
    }
}
