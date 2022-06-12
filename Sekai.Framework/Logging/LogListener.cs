// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Platform;

namespace Sekai.Framework.Logging;

public abstract class LogListener : FrameworkObject
{
    public int MessagesLogged { get; private set; }
    public LogLevel Level { get; set; } = RuntimeInfo.IsDebug ? LogLevel.Debug : LogLevel.Information;

    private int logEveryCount = 1;
    public int LogEveryCount
    {
        get => logEveryCount;
        set => logEveryCount = Math.Max(value, 1);
    }


    protected abstract void OnNewMessage(string message);

    protected virtual void Flush()
    {
    }

    protected virtual Func<DateTime, string> FormatTimestamp { get; } = t => t.ToString("dd/MM/yyyy hh:mm:ss tt");
    protected virtual Func<LogLevel, string> FormatLogLevel { get; } = l => l.ToString();
    protected virtual Func<string, string> FormatChannel { get; } = c => c;
    protected virtual Func<string, string> FormatMessage { get; } = m => m;
    protected virtual Func<Exception, string> FormatException { get; } = e => e.ToString();

    protected virtual string GetTextFormatted(LogMessage message)
    {
        return $"[{FormatTimestamp(message.Timestamp)}] [{FormatLogLevel(message.Level)}]{(string.IsNullOrEmpty(message.Channel) ? string.Empty : $" [{FormatChannel(message.Channel)}]")} {FormatMessage(message.Message)}{(message.Exception != null ? $"\n{FormatException(message.Exception)}" : string.Empty)}";
    }

    private void handleNewMessage(LogMessage message)
    {
        if (message.Level < Level)
            return;

        OnNewMessage(GetTextFormatted(message));
        MessagesLogged++;

        if ((MessagesLogged % LogEveryCount) == 0)
            Flush();
    }

    public static implicit operator Action<LogMessage>(LogListener listener)
    {
        return listener.handleNewMessage;
    }
}
