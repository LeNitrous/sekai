// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Logging;

internal sealed class LogWriterTrace : LogWriter
{
    public LogWriterTrace()
    {
        Trace.AutoFlush = false;
    }

    protected override void Flush()
    {
        Trace.Flush();
    }

    protected override void Write(LogMessage message)
    {
        if (message.Level == LogLevel.Trace)
        {
            Trace.WriteLine(message);
        }
    }
}
