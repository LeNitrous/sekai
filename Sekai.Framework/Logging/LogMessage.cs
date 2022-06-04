// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Logging;

public struct LogMessage
{
    public readonly DateTime Timestamp;
    public readonly LogLevel Level;
    public readonly string? Channel;
    public readonly string Message;
    public readonly CallerInfo CallerInfo;
    public readonly Exception? Exception;

    public LogMessage(string message, string? channel = null, LogLevel level = LogLevel.Verbose, Exception? exception = null, CallerInfo callerInfo = default)
    {
        Level = level;
        Channel = channel;
        Message = message;
        Timestamp = DateTime.Now;
        Exception = exception;
        CallerInfo = callerInfo;
    }
}
