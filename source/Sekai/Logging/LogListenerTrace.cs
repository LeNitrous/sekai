// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;

namespace Sekai.Logging;

/// <summary>
/// A log listener that writes to <see cref="Trace"/>.
/// </summary>
public class LogListenerTrace : LogListener
{
    private static LogListenerTrace? current;

    public LogListenerTrace()
    {
        if (current is not null)
            throw new InvalidOperationException($"There is already an existing instance of a {nameof(LogListenerTrace)}.");

        current = this;
        Trace.AutoFlush = false;
    }

    protected override void Write(string message)
    {
        Trace.WriteLine(message);
    }

    protected override void Flush()
    {
        Trace.Flush();
    }

    protected override void Destroy()
    {
        current = null;
    }
}
