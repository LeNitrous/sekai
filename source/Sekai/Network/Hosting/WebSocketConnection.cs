// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Threading;

namespace Sekai.Network.Hosting;

public abstract class WebSocketConnection : DisposableObject
{
    /// <summary>
    /// Invoked when the websocket has started.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? OnStart;

    /// <summary>
    /// Invoked when the websocket has closed.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? OnClose;

    /// <summary>
    /// Invoked when the websocket is disconnected.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? OnDisconnect;

    /// <summary>
    /// Invoked when the websocket receives a message.
    /// </summary>
    public event AsyncEventHandler<WebSocketMessageEventArgs>? OnMessage;

    protected readonly WebSocket Socket;

    private Task? runTask;
    private CancellationTokenSource? tokenClose;

    protected WebSocketConnection(WebSocket socket)
    {
        Socket = socket;
    }

    /// <summary>
    /// Sends a message as a string to the remote destination.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task SendAsync(string message, CancellationToken token = default)
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(message.Length);
        int count = Encoding.UTF8.GetBytes(message, buffer.Memory.Span);
        await Socket.SendAsync(buffer.Memory[..count], WebSocketMessageType.Text, true, token);
    }

    /// <summary>
    /// Sends a message as binary data to the remote destination.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken token = default)
    {
        await Socket.SendAsync(data, WebSocketMessageType.Binary, true, token);
    }

    /// <summary>
    /// Starts the websocket message loop.
    /// </summary>
    public void Start()
    {
        if (runTask is not null || tokenClose is not null)
            return;

        tokenClose = new();
        runTask = Task.Factory.StartNew(runMessageLoop, tokenClose.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// Stops the websocket message loop and closes the socket.
    /// </summary>
    public void Close()
    {
        Close(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Stops the websocket message loop and closes the socket.
    /// </summary>
    /// <param name="timeout">The timeout time.</param>
    public void Close(TimeSpan timeout)
    {
        using var tokenTimeout = new CancellationTokenSource(timeout);
        CloseAsync(tokenTimeout.Token).Wait(tokenTimeout.Token);
    }

    /// <summary>
    /// Stops the websocket message loop and closes the socket asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token</param>
    public async Task CloseAsync(CancellationToken token = default)
    {
        if (runTask is null || tokenClose is null)
            return;

        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);

        tokenClose.Cancel();

        await runTask;

        tokenClose.Dispose();
        tokenClose = null;

        runTask = null;
    }

    private async Task runMessageLoop()
    {
        if (tokenClose is null)
            throw new InvalidOperationException(@"Failed to start websocket message loop.");

        var token = tokenClose.Token;
        var buffer = MemoryPool<byte>.Shared.Rent(4096);

        try
        {
            int read = 0;

            await (OnStart?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);

            while (!token.IsCancellationRequested)
            {
                // Occurs when the close handshake has been completed.
                if (Socket.State == WebSocketState.Closed)
                {
                    await (OnDisconnect?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);
                    break;
                }

                // This call locks so we handle everything else before we poll for an incoming message.
                var received = await Socket.ReceiveAsync(buffer.Memory[read..], token);
                read += received.Count;

                // Occurs if we have aborted the socket by token cancellation.
                if (Socket.State == WebSocketState.Aborted)
                {
                    await (OnDisconnect?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);
                    break;
                }

                // Occurs when remote is notifying us that the connection will close.
                if (Socket.State == WebSocketState.CloseReceived && received.MessageType == WebSocketMessageType.Close)
                {
                    await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, token);
                    continue;
                }

                // Occurs if the last message frame is the end.
                if (Socket.State == WebSocketState.Open && received.EndOfMessage)
                {
                    var e = new WebSocketMessageEventArgs(buffer.Memory[..read]);

                    await (OnMessage?.InvokeAsync(this, e) ?? Task.CompletedTask);
                    read = 0;

                    if (e.Close)
                        await Socket.CloseAsync(e.CloseStatus, e.CloseReason, token);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (WebSocketException)
        {
        }
        finally
        {
            buffer.Dispose();

            await (OnClose?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        Close();
        Socket.Dispose();
    }
}
