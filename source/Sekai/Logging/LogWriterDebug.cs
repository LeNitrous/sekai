// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Logging;

internal sealed class LogWriterDebug : LogWriter
{
    public LogWriterDebug()
    {
        Debug.AutoFlush = false;
    }

    protected override void Flush()
    {
        Debug.Flush();
    }

    protected override void Write(LogMessage message)
    {
        if (message.Level == LogLevel.Debug)
        {
            Debug.WriteLine(message);
        }
    }
}
