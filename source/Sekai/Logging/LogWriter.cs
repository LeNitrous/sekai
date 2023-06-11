// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Logging;

/// <summary>
/// Writes <see cref="LogMessage"/>s to output sources.
/// </summary>
public abstract class LogWriter
{
    /// <summary>
    /// The frequency of logging.
    /// </summary>
    public int Frequency;

    private int messagesLogged;
    private readonly object syncLock = new();

    public void Receive(LogMessage message)
    {
        lock (syncLock)
        {
            Write(message);

            messagesLogged++;

            if ((messagesLogged % Frequency) == 0)
            {
                Flush();
            }
        }
    }

    /// <summary>
    /// Flushes the contents of this <see cref="LogWriter"/>.
    /// </summary>
    protected abstract void Flush();

    /// <summary>
    /// Writes a <see cref="LogMessage"/> to this writer.
    /// </summary>
    /// <param name="message"></param>
    protected abstract void Write(LogMessage message);
}
