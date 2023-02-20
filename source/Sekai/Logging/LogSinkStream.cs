// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using System.Text;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="Stream"/>backed <see cref="LogSink"/>.
/// </summary>
public class LogSinkStream : LogSink
{
    private readonly Stream stream;

    public LogSinkStream(Stream stream)
    {
        if (!stream.CanWrite)
            throw new ArgumentException("The stream must be writable.", nameof(stream));

        this.stream = stream;
    }

    public override void Clear()
    {
        stream.SetLength(0);
    }

    public override void Flush()
    {
        stream.Flush();
    }

    protected override void Write(string message)
    {
        Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(message) + 1];
        Encoding.UTF8.GetBytes(message, buffer);
        buffer[^1] = 0x0A;
        stream.Write(buffer);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            stream.Dispose();
    }
}
