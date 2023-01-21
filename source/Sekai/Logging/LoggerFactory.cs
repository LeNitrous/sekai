// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

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
            logger = new Logger(name);
            loggers.Add(name, logger);

            foreach (var listener in listeners)
                logger.OnMessageLogged += listener;
        }

        return logger;
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
