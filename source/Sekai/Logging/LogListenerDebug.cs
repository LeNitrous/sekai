// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;

namespace Sekai.Logging;

/// <summary>
/// A log listener that writes to <see cref="Debug"/>.
/// </summary>
public class LogListenerDebug : LogListener
{
    private static LogListenerDebug? current;

    public LogListenerDebug()
    {
        if (current is not null)
            throw new InvalidOperationException($"There is already an existing instance of a {nameof(LogListenerDebug)}.");

        current = this;
        Debug.AutoFlush = false;
    }

    protected override void Write(string message)
    {
        Debug.WriteLine(message);
    }

    protected override void Flush()
    {
        Debug.Flush();
    }

    protected override void Destroy()
    {
        current = null;
    }
}
