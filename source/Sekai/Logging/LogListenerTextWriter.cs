// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace Sekai.Logging;

public class LogListenerTextWriter : LogListener
{
    private readonly TextWriter writer;

    public LogListenerTextWriter(Stream stream)
    {
        writer = new StreamWriter(stream);
    }

    public LogListenerTextWriter(TextWriter writer)
    {
        this.writer = writer;
    }

    protected override void OnNewMessage(string message)
    {
        if (!IsDisposed)
        {
            lock (writer)
                writer.WriteLine(message);
        }
    }

    protected override void Flush()
    {
        if (!IsDisposed)
            writer.Flush();
    }

    protected override void Destroy()
    {
        if (!IsDisposed)
            writer.Dispose();
    }
}
