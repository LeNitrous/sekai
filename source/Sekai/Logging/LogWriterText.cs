// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogWriter"/> that writes text.
/// </summary>
public class LogWriterText : LogWriterStream
{
    private readonly Stream stream;
    private readonly Encoding encoding;

    public LogWriterText(Stream stream, Encoding? encoding = null, bool leaveOpen = false)
        : base(stream, leaveOpen)
    {
        this.stream = stream;
        this.encoding = encoding ?? Encoding.UTF8;
    }

    public sealed override void Flush()
    {
        stream.Flush();
    }

    public sealed override void Write(LogMessage message)
    {
        string formatted = Format(message);

        Span<byte> buffer = stackalloc byte[encoding.GetByteCount(formatted)];

        if (encoding.GetBytes(formatted.AsSpan(), buffer) <= 0)
        {
            return;
        }

        stream.Write(buffer);
    }

    protected virtual string Format(LogMessage m) => string.Format(m.Exception is null ? format : format_with_exception, GetLevel(m), GetTimestamp(m), GetContent(m), GetException(m));
    protected virtual string? GetLevel(LogMessage m) => designators[m.Level];
    protected virtual string? GetContent(LogMessage m) => (m.Content is null && m.Exception is not null) ? format_exception_message : m.Content?.ToString();
    protected virtual string? GetTimestamp(LogMessage m) => m.Timestamp.ToString(format_timestamp);
    protected virtual string? GetException(LogMessage m) => m.Exception?.ToString();

    private const string format_timestamp = "dd/MM/yyyy hh:mm:ss tt";
    private const string format_exception_message = "An exception has occured.";
    private const string format_header = "[{0}] [{1}]";
    private const string format_content = "{2}";
    private const string format_content_with_exception = "{2}\n{3}";
    private const string format = $"{format_header} {format_content}\n";
    private const string format_with_exception = $"{format_header} {format_content}\n{format_content_with_exception}\n";

    private static readonly Dictionary<LogLevel, string> designators = new()
    {
        { LogLevel.Trace, "TRC" },
        { LogLevel.Debug, "DBG" },
        { LogLevel.Verbose, "INF" },
        { LogLevel.Warning, "WRN" },
        { LogLevel.Error, "ERR" },
        { LogLevel.Critical, "CRT" },
    };
}
