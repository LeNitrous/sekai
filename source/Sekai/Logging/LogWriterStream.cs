// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using System.Text;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogWriter"/> that uses a <see cref="Stream"/>.
/// </summary>
public abstract class LogWriterStream : LogWriter, IDisposable
{
    private bool isDisposed;
    private readonly Stream stream;
    private readonly Encoding encoding;

    protected LogWriterStream(Stream stream, Encoding? encoding = null)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("The stream does not support writing.", nameof(stream));
        }

        this.stream = stream;
        this.encoding = encoding ?? Encoding.UTF8;
    }

    protected sealed override void Flush()
    {
        if (isDisposed)
        {
            return;
        }

        stream.Flush();
    }

    protected sealed override void Write(LogMessage message)
    {
        if (isDisposed)
        {
            return;
        }

        string formatted = Format(message);
        Span<byte> buffer = stackalloc byte[encoding.GetByteCount(formatted)];

        if (encoding.GetBytes(formatted.AsSpan(), buffer) > 0)
        {
            stream.Write(buffer);
        }
    }

    /// <summary>
    /// Formats a <see cref="LogMessage"/> as a <see cref="string"/>.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <returns>A formatted <see cref="LogMessage"/>.</returns>
    protected abstract string Format(LogMessage message);

    ~LogWriterStream()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        stream.Dispose();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
