// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Framework.Logging;

internal sealed class LogWriterDebug : LogWriter
{
    public LogWriterDebug()
    {
        Debug.AutoFlush = false;
    }

    public override void Flush()
    {
        Debug.Flush();
    }

    public override void Write(LogMessage message)
    {
        if (message.Level == LogLevel.Debug)
        {
            Debug.WriteLine(message);
        }
    }
}
