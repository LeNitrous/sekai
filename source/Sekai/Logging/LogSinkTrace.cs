// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogSink"/> that writes to the <see cref="Trace"/> output.
/// </summary>
public sealed class LogSinkTrace : LogSink
{
    public LogSinkTrace()
    {
        Trace.AutoFlush = false;
    }

    public override void Clear()
    {
        // Trace does not support clearing.
    }

    public override void Flush()
    {
        Trace.Flush();
    }

    protected override void Write(string message)
    {
        Trace.WriteLine(message);
    }
}
