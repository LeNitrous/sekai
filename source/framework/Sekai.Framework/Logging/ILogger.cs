// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Logging;

public interface ILogger
{
    event Action<LogMessage> OnMessageLogged;
    void Log(object message, LogLevel level = LogLevel.Verbose);
    void Debug(object message);
    void Verbose(object message);
    void Info(object message);
    void Warning(object message);
    void Error(object message, Exception? exception = null);
}
