// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogSink"/> that writes to the <see cref="Debug"/> output.
/// </summary>
public sealed class LogSinkDebug : LogSink
{
    public LogSinkDebug()
    {
        Debug.AutoFlush = false;
    }

    public override void Clear()
    {
        // Debug does not support clearing.
    }

    public override void Flush()
    {
        Debug.Flush();
    }

    protected override void Write(string message)
    {
        Debug.WriteLine(message);
    }
}
