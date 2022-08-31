// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Logging;

public abstract class LogListener : FrameworkObject
{
    /// <summary>
    /// Gets the number of messages logged by this listener.
    /// </summary>
    public int MessagesLogged { get; private set; }

    /// <summary>
    /// Gets or sets the minimum logging level the listener can listen to.
    /// </summary>
    public LogLevel Level { get; set; } = RuntimeInfo.IsDebug ? LogLevel.Debug : LogLevel.Information;

    /// <summary>
    /// Gets or sets the filtering function for this listener. Return true to filter the message.
    /// </summary>
    public Func<LogMessage, bool> Filter = null!;

    private int logEveryCount = 1;

    /// <summary>
    /// Gets or sets the how often the listener should flush.
    /// </summary>
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
        if (message.Level < Level || (Filter?.Invoke(message) ?? false))
            return;

        OnNewMessage(GetTextFormatted(message));
        MessagesLogged++;

        if ((MessagesLogged % LogEveryCount) == 0)
            Flush();
    }

    public static implicit operator Action<LogMessage>(LogListener listener) => listener.handleNewMessage;
}
