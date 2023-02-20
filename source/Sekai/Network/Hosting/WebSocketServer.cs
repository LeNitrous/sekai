// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Threading;

namespace Sekai.Network.Hosting;

public class WebSocketServer : DisposableObject
{
    /// <summary>
    /// Invoked when the server starts.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? OnStart;

    /// <summary>
    /// Invoked when the server closes.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? OnClose;

    /// <summary>
    /// Invoked when a client has been connected.
    /// </summary>
    public event AsyncEventHandler<WebSocketConnectionEventArgs>? OnClientConnect;

    /// <summary>
    /// Invoked when a client has been disconnected.
    /// </summary>
    public event AsyncEventHandler<WebSocketConnectionEventArgs>? OnClientDisconnect;

    /// <summary>
    /// Invoked when the server receives a message from a connected client.
    /// </summary>
    public event AsyncEventHandler<WebSocketConnectionMessageEventArgs>? OnClientMessage;

    private Task? runTask;
    private CancellationTokenSource? tokenClose;
    private readonly Uri uri;
    private readonly ConcurrentDictionary<Guid, WebSocketConnection> connections = new();

    public WebSocketServer(Uri uri)
    {
        this.uri = uri;
    }

    /// <summary>
    /// Sends a message as binary data to all connected clients.
    /// </summary>
    /// <param name="data">The data to be sent.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken token = default)
    {
        await Task.WhenAll(connections.Values.Select(c => c.SendAsync(data, token)));
    }

    /// <summary>
    /// Sends a message as a string to all connected clients.
    /// </summary>
    /// <param name="data">The data to be sent.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task SendAsync(string data, CancellationToken token = default)
    {
        await Task.WhenAll(connections.Values.Select(c => c.SendAsync(data, token)));
    }

    /// <summary>
    /// Starts the websocket server.
    /// </summary>
    public void Start()
    {
        if (runTask is not null || tokenClose is not null)
            return;

        tokenClose = new();
        runTask = Task.Factory.StartNew(runServerLoop, tokenClose.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// Closes the websocket server.
    /// </summary>
    public void Close()
    {
        Close(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Closes the websocket server within the given time limit.
    /// </summary>
    /// <param name="timeout">The time limit before timing out.</param>
    public void Close(TimeSpan timeout)
    {
        if (runTask is null || tokenClose is null)
            return;

        tokenClose.Cancel();

        using var tokenTimeout = new CancellationTokenSource(timeout);
        runTask.Wait(tokenTimeout.Token);

        tokenClose.Dispose();
        tokenClose = null;

        runTask = null;
    }

    private async Task runServerLoop()
    {
        if (tokenClose is null)
            return;

        var token = tokenClose.Token;

        string url = uri.OriginalString;
        url = !url.EndsWith('/') ? url + '/' : url;

        var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        await (OnStart?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);

        while (!token.IsCancellationRequested)
        {
            if (!listener.IsListening)
                break;

            var context = await listener.GetContextAsync();

            if (context.Request.Url is null)
            {
                // Bad Request
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
            else
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                var ws = new WebSocketServerConnection(Guid.NewGuid(), wsContext.WebSocket);
                ws.OnStart += handleWebSocketStart;
                ws.OnClose += handleWebSocketClose;
                ws.OnMessage += handleWebSocketMessage;
                ws.Start();
            }
        }

        await Task.WhenAll(connections.Values.Select(c => c.CloseAsync()));
        await (OnClose?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);

        listener.Stop();
        listener.Close();
    }

    private async Task handleWebSocketStart(object? sender, EventArgs e)
    {
        if (sender is not WebSocketServerConnection ws)
            return;

        connections.TryAdd(ws.Guid, ws);
        await (OnClientConnect?.InvokeAsync(this, new(ws.Guid, ws)) ?? Task.CompletedTask);
    }

    private async Task handleWebSocketClose(object? sender, EventArgs e)
    {
        if (sender is not WebSocketServerConnection ws)
            return;

        connections.TryRemove(ws.Guid, out _);

        ws.OnStart -= handleWebSocketStart;
        ws.OnClose -= handleWebSocketClose;
        ws.OnMessage -= handleWebSocketMessage;

        await (OnClientDisconnect?.InvokeAsync(this, new(ws.Guid, ws)) ?? Task.CompletedTask);

        ws.Dispose();
    }

    private async Task handleWebSocketMessage(object? sender, WebSocketMessageEventArgs e)
    {
        if (sender is not WebSocketServerConnection ws)
            return;

        await (OnClientMessage?.InvokeAsync(this, new(ws.Guid, ws, e)) ?? Task.CompletedTask);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Close();
    }

    private class WebSocketServerConnection : WebSocketConnection
    {
        public readonly Guid Guid;

        public WebSocketServerConnection(Guid guid, WebSocket socket)
            : base(socket)
        {
            Guid = guid;
        }
    }
}
