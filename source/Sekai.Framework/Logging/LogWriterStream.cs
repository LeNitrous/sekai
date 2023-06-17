// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sekai.Framework.Logging;

/// <summary>
/// A <see cref="LogWriter"/> that uses a <see cref="Stream"/>.
/// </summary>
public class LogWriterStream : LogWriter, IDisposable
{
    private bool isDisposed;
    private readonly Stream stream;
    private readonly Encoding encoding;

    public LogWriterStream(Stream stream, Encoding? encoding = null)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("The stream does not support writing.", nameof(stream));
        }

        this.stream = stream;
        this.encoding = encoding ?? Encoding.UTF8;
    }

    public sealed override void Flush()
    {
        if (isDisposed)
        {
            return;
        }

        stream.Flush();
    }

    public sealed override void Write(LogMessage message)
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
    protected virtual string Format(LogMessage message)
    {
        var builder = new StringBuilder();
        builder.Append('[');
        builder.Append(labels[message.Level]);
        builder.Append(']');
        builder.Append(' ');
        builder.Append('[');
        builder.Append(message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss tt"));

        if (message.Content is not null)
        {
            builder.Append(' ');
            builder.Append(message.Content);
        }

        if (message.Exception is not null)
        {
            if (message.Content is not null)
            {
                builder.AppendLine();
            }
            else
            {
                builder.Append(' ');
                builder.Append("An exception has occured.");
            }

            builder.Append(message.Exception);
        }

        builder.AppendLine();

        return builder.ToString();
    }

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

    private static readonly Dictionary<LogLevel, string> labels = new()
    {
        { LogLevel.Trace,       "TRC" },
        { LogLevel.Debug,       "DBG" },
        { LogLevel.Information, "INF" },
        { LogLevel.Warning,     "WRN" },
        { LogLevel.Error,       "ERR" },
        { LogLevel.Critical,    "CRT" },
    };
}
