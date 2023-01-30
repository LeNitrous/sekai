// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Network.Hosting;

/// <summary>
/// Event data for a websocket connection event.
/// </summary>
public class WebSocketConnectionEventArgs : EventArgs
{
    /// <summary>
    /// The connection unique identifier.
    /// </summary>
    public readonly Guid ConnectionId;

    /// <summary>
    /// The connecting websocket.
    /// </summary>
    public readonly WebSocketConnection WebSocket;

    public WebSocketConnectionEventArgs(Guid connectionId, WebSocketConnection webSocket)
    {
        ConnectionId = connectionId;
        WebSocket = webSocket;
    }
}
