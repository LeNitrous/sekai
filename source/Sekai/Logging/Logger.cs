// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Logging;

internal sealed class Logger : DisposableObject, ILogger
{
    private readonly string[] tags;
    private readonly IReadOnlyList<LogSink> sinks;

    public Logger(IReadOnlyList<LogSink> sinks)
        : this(sinks, Array.Empty<string>())
    {
    }

    private Logger(IReadOnlyList<LogSink> sinks, string[] tags)
    {
        this.tags = tags;
        this.sinks = sinks;
    }

    public ILogger GetLogger(string category) => new Logger(sinks, tags.Concat(new[] { category }).ToArray());

    public void Log(object? message, params object?[] args) => log(new(message, args, LogLevel.Verbose, tags));

    public void Debug(object? message, params object?[] args) => log(new(message, args, LogLevel.Debug, tags));

    public void Info(object? message, params object?[] args) => log(new(message, args, LogLevel.Information, tags));

    public void Warn(object? message, params object?[] args) => log(new(message, args, LogLevel.Warning, tags));

    public void Error(object? message, Exception? exception = null, params object?[] args) => log(new(message, args, LogLevel.Error, tags, exception));

    private void log(LogMessage message)
    {
        foreach (var sink in sinks)
            sink.Write(message);
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var sink in sinks)
            sink.Dispose();
    }
}
