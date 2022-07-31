// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace Sekai.Framework.Logging;

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
        lock (writer)
            writer.WriteLine(message);
    }

    protected override void Flush()
    {
        writer.Flush();
    }

    protected override void Destroy()
    {
        writer.Dispose();
    }
}
