// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

/// <summary>
/// Listens to <see cref="Logger"/> output.
/// </summary>
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
    public Func<LogMessage, bool> Filter = m => false;

    private int logEveryCount = 1;

    /// <summary>
    /// Gets or sets the how often the listener should flush.
    /// </summary>
    public int LogEveryCount
    {
        get => logEveryCount;
        set => logEveryCount = Math.Max(value, 1);
    }

    /// <summary>
    /// Clears messages logged by this listener.
    /// </summary>
    public virtual void Clear()
    {
        MessagesLogged = 0;
    }

    /// <summary>
    /// Writes the <paramref name="message"/> to its output.
    /// </summary>
    /// <param name="message">The formatted message.</param>
    protected abstract void Write(string message);

    /// <summary>
    /// Flushes written messages to its output.
    /// </summary>
    protected virtual void Flush()
    {
    }

    /// <summary>
    /// The <see cref="DateTime"/> formatter.
    /// </summary>
    protected virtual Func<DateTime, string> FormatTimestamp { get; } = t => t.ToString("dd/MM/yyyy hh:mm:ss tt");

    /// <summary>
    /// The <see cref="LogLevel"/> formatter.
    /// </summary>
    protected virtual Func<LogLevel, string> FormatLogLevel { get; } = l => l.ToString();

    /// <summary>
    /// The channel text formatter.
    /// </summary>
    protected virtual Func<string, string> FormatChannel { get; } = c => c;

    /// <summary>
    /// The message formatter.
    /// </summary>
    protected virtual Func<object?, string> FormatMessage { get; } = m => m?.ToString() ?? "null";

    /// <summary>
    /// The <see cref="Exception"/> formatter.
    /// </summary>
    protected virtual Func<Exception, string> FormatException { get; } = e => e.ToString();

    /// <summary>
    /// Formats a <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="message">The message to be formatted.</param>
    /// <returns></returns>
    protected virtual string GetMessageFormatted(LogMessage message)
    {
        return $"[{FormatTimestamp(message.Timestamp)}] [{FormatLogLevel(message.Level)}]{(string.IsNullOrEmpty(message.Channel) ? string.Empty : $" [{FormatChannel(message.Channel)}]")} {FormatMessage(message.Message)}{(message.Exception != null ? $"\n{FormatException(message.Exception)}" : string.Empty)}";
    }

    private void handleNewMessage(LogMessage message)
    {
        if (message.Level < Level || Filter(message))
            return;

        Write(GetMessageFormatted(message));
        MessagesLogged++;

        if ((MessagesLogged % LogEveryCount) == 0)
            Flush();
    }

    public static implicit operator Action<LogMessage>(LogListener listener) => listener.handleNewMessage;
}
