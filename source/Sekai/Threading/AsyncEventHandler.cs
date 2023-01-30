// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sekai.Threading;

public delegate Task AsyncEventHandler<T>(object? sender, T args) where T : EventArgs;

public class AsyncEventArgs : EventArgs
{
    /// <summary>
    /// The associated cancellation token.
    /// </summary>
    public readonly CancellationToken Token;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token">The associated cancellation token.</param>
    public AsyncEventArgs(CancellationToken token)
    {
        Token = token;
    }

    /// <summary>
    /// 
    /// </summary>
    public AsyncEventArgs()
        : this(default)
    {
    }
}

public static class AsyncEventHandlerExtensions
{
    public static Task InvokeAsync<T>(this AsyncEventHandler<T> handler, object? sender, T args)
        where T : EventArgs
    {
        return Task.WhenAll(getHandlers(handler).Select(func => func(sender, args)));
    }

    private static IEnumerable<AsyncEventHandler<T>> getHandlers<T>(AsyncEventHandler<T> handler)
        where T : EventArgs
    {
        return handler.GetInvocationList().Cast<AsyncEventHandler<T>>();
    }
}
