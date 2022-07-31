// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Framework.Logging;

public class LogListenerTrace : LogListener
{
    public LogListenerTrace()
    {
        Trace.AutoFlush = false;
    }

    protected override void OnNewMessage(string message)
    {
        Trace.WriteLine(message);
    }

    protected override void Flush()
    {
        Trace.Flush();
    }
}
