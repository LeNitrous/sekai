// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Framework.Logging;

internal sealed class LogWriterTrace : LogWriter
{
    public LogWriterTrace()
    {
        Trace.AutoFlush = false;
    }

    public override void Flush()
    {
        Trace.Flush();
    }

    public override void Write(LogMessage message)
    {
        if (message.Level == LogLevel.Trace)
        {
            Trace.WriteLine(message);
        }
    }
}
