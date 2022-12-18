// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Logging;

public class LogListenerDebug : LogListener
{
    public LogListenerDebug()
    {
        Debug.AutoFlush = false;
    }

    protected override void OnNewMessage(string message)
    {
        Debug.WriteLine(message);
    }

    protected override void Flush()
    {
        Debug.Flush();
    }
}
