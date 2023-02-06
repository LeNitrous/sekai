// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogListener"/> with an underlying <see cref="Stream"/>.
/// </summary>
public abstract class LogListenerStream : LogListener
{
    private readonly Stream stream;

    public LogListenerStream(Stream stream)
    {
        this.stream = stream;
    }

    public override void Clear()
    {
        base.Clear();

        if (stream.CanSeek)
            stream.SetLength(0);
    }

    protected override void Flush()
    {
        stream.Flush();
    }
}
