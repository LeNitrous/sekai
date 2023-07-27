// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogWriter"/> that uses a <see cref="Stream"/>.
/// </summary>
public abstract class LogWriterStream : LogWriter
{
    private bool isDisposed;
    private readonly bool leaveOpen;
    private readonly Stream stream;

    public LogWriterStream(Stream stream, bool leaveOpen = false)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("The stream does not support writing.", nameof(stream));
        }

        this.stream = stream;
        this.leaveOpen = leaveOpen;
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Flush();

        if (!leaveOpen)
        {
            stream.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
