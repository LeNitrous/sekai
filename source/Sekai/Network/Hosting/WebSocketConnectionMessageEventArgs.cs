// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Net.WebSockets;

namespace Sekai.Network.Hosting;

/// <summary>
/// Event data for a websocket message event.
/// </summary>
public class WebSocketConnectionMessageEventArgs : EventArgs
{
    /// <summary>
    /// The connection unique identifier.
    /// </summary>
    public readonly Guid ConnectionId;

    /// <summary>
    /// The connecting websocket.
    /// </summary>
    public readonly WebSocketConnection WebSocket;

    public bool Close
    {
        get => messageEvent.Close;
        set => messageEvent.Close = value;
    }

    public WebSocketCloseStatus CloseStatus
    {
        get => messageEvent.CloseStatus;
        set => messageEvent.CloseStatus = value; 
    }

    public string? CloseReason
    {
        get => messageEvent.CloseReason;
        set => messageEvent.CloseReason = value;
    }

    public ReadOnlyMemory<byte> Buffer => messageEvent.Buffer;

    private readonly WebSocketMessageEventArgs messageEvent;

    public WebSocketConnectionMessageEventArgs(Guid connectionId, WebSocketConnection webSocket, WebSocketMessageEventArgs e)
    {
        ConnectionId = connectionId;
        WebSocket = webSocket;
        messageEvent = e;
    }
}
