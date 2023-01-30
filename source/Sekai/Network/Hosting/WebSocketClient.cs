// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sekai.Network.Hosting;

/// <summary>
/// A websocket capable of connecting to a remote server.
/// </summary>
public class WebSocketClient : WebSocketConnection
{
    protected new ClientWebSocket Socket => (ClientWebSocket)base.Socket;

    private readonly Uri uri;
    private readonly TimeSpan connectionTimeout;

    public WebSocketClient(Uri uri, TimeSpan connectionTimeout)
        : base(new ClientWebSocket())
    {
        this.uri = uri;
        this.connectionTimeout = connectionTimeout;
        OnStart += handleStartup;
    }

    public WebSocketClient(Uri uri)
        : this(uri, TimeSpan.FromSeconds(10))
    {
    }

    private async Task handleStartup(object? sender, EventArgs e)
    {
        using var timeout = new CancellationTokenSource(connectionTimeout);
        await Socket.ConnectAsync(uri, timeout.Token);
    }

    protected override void Destroy()
    {
        OnStart -= handleStartup;
        base.Destroy();
    }
}
