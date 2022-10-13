// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Logging;

public partial class Logger
{
    public static event Action<LogMessage>? OnMessageLogged;
    private static readonly Logger global = new();
    private static readonly Dictionary<string, Logger> loggers = new();

    public static void Debug(object message)
    {
        global.Debug(message);
    }

    public static void Error(object message, Exception? exception = null)
    {
        global.Error(message, exception);
    }

    public static void Info(object message)
    {
        global.Info(message);
    }

    public static void Log(object message, LogLevel level = LogLevel.Verbose)
    {
        global.Log(message, level);
    }

    public static void Verbose(object message)
    {
        global.Verbose(message);
    }

    public static void Warning(object message)
    {
        global.Warning(message);
    }

    public static Logger GetLogger(string name)
    {
        if (!loggers.TryGetValue(name, out var logger))
        {
            logger = new Logger(name);
            loggers.Add(name, logger);
        }

        return logger;
    }
}
