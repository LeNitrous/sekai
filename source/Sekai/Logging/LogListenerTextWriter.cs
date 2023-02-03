// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace Sekai.Logging;

/// <summary>
/// A log listener backed by a <see cref="Stream"/> with an underlying <see cref="TextWriter"/>.
/// </summary>
public class LogListenerTextWriter : LogListenerStream
{
    protected readonly TextWriter Writer;

    public LogListenerTextWriter(Stream stream)
        : base(stream)
    {
        Writer = new StreamWriter(stream);
    }

    protected override void Write(string message)
    {
        if (!IsDisposed)
        {
            lock (Writer)
                Writer.WriteLine(message);
        }
    }

    protected override void Flush()
    {
        // Intentionally not calling base implementation as the writer flushes its underlying stream internally.
        if (!IsDisposed)
            Writer.Flush();
    }

    protected override void Destroy()
    {
        Writer.Dispose();
        base.Destroy();
    }
}
