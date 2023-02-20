// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Logging;

/// <summary>
/// An <see cref="ILogger"/> output destination.
/// </summary>
public abstract class LogSink : DisposableObject
{
    /// <summary>
    /// The number of messages written to this sink.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// The minimum level of messages allowed to be written to this sink.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// The frequency of messages being flushed in this sink.
    /// </summary>
    public int Frequency
    {
        get => frequency;
        set => frequency = Math.Max(value, 1);
    }

    /// <summary>
    /// The filtering function of this sink.
    /// </summary>
    /// <remarks>
    /// Return true to filter the message. Otherwise return false.
    /// </remarks>
    public Func<LogMessage, bool> Filter = m => false;

    /// <summary>
    /// The log formatter for this sink.
    /// </summary>
    public LogFormatter Format = LogFormatter.Default;

    private int frequency = 1;
    private readonly object syncLock = new();

    /// <summary>
    /// Writes a message to this sink.
    /// </summary>
    /// <param name="message">The message to be written.</param>
    public void Write(LogMessage message)
    {
        lock (syncLock)
        {
            if (message.Level < Level || Filter(message))
                return;

            Write(Format.Format(message));
            Count++;

            if ((Count % Frequency) == 0)
                Flush();
        }
    }

    public abstract void Flush();

    public abstract void Clear();

    protected abstract void Write(string message);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Flush();
    }
}
