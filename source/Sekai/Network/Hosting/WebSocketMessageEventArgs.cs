// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Net.WebSockets;

namespace Sekai.Network.Hosting;

/// <summary>
/// Data for a websocket message event.
/// </summary>
public class WebSocketMessageEventArgs : EventArgs
{
    /// <summary>
    /// Whether to close the websocket or not.
    /// </summary>
    public bool Close;

    /// <summary>
    /// The close reason message.
    /// </summary>
    public string? CloseReason;

    /// <summary>
    /// The close status.
    /// </summary>
    public WebSocketCloseStatus CloseStatus;

    /// <summary>
    /// The message buffer.
    /// </summary>
    public readonly ReadOnlyMemory<byte> Buffer;

    public WebSocketMessageEventArgs(ReadOnlyMemory<byte> buffer)
    {
        Buffer = buffer;
    }
}
