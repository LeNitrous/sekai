// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Logging;

public interface ILogger
{
    event Action<LogMessage> OnMessageLogged;
    void Log(string message, LogLevel level = LogLevel.Verbose);
    void Debug(string message);
    void Verbose(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? exception = null);
}
