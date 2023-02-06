// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Logging;

public sealed class LoggerFactory : FrameworkObject
{
    private readonly Logger global = new();
    private readonly HashSet<LogListener> listeners = new();
    private readonly Dictionary<string, Logger> loggers = new();

    /// <summary>
    /// Adds a log listener.
    /// </summary>
    public void Add(LogListener listener)
    {
        listeners.Add(listener);

        foreach (var logger in loggers.Values)
            logger.OnMessageLogged += listener;

        global.OnMessageLogged += listener;
    }

    /// <summary>
    /// Removes a log listener.
    /// </summary>
    public void Remove(LogListener listener)
    {
        listeners.Remove(listener);

        foreach (var logger in loggers.Values)
            logger.OnMessageLogged -= listener;

        global.OnMessageLogged -= listener;
    }

    /// <summary>
    /// Gets a logger with a given name.
    /// </summary>
    /// <param name="name">The logger with a given name. If null, the global instance is returned.</param>
    public Logger GetLogger(string? name = null)
    {
        if (string.IsNullOrEmpty(name))
            return global;

        if (!loggers.TryGetValue(name, out var logger))
        {
            logger = new Logger(this, name);
            loggers.Add(name, logger);

            foreach (var listener in listeners)
                logger.OnMessageLogged += listener;
        }

        return logger;
    }

    internal void RemoveLogger(Logger logger)
    {
        if (string.IsNullOrEmpty(logger.Name))
            throw new ArgumentException(@"Failed to remove logger.", nameof(logger));

        if (!loggers.Remove(logger.Name))
            return;

        foreach (var listener in listeners)
            logger.OnMessageLogged -= listener;
    }

    protected override void Destroy()
    {
        foreach (var listener in listeners)
        {
            foreach (var logger in loggers.Values)
                logger.OnMessageLogged -= listener;

            global.OnMessageLogged -= listener;
        }

        loggers.Clear();
        listeners.Clear();
    }
}
