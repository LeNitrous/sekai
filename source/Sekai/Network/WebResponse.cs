// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Text;
using CommunityToolkit.HighPerformance;

namespace Sekai.Network;

/// <summary>
/// A web response.
/// </summary>
public sealed class WebResponse : DisposableObject
{
    /// <summary>
    /// The status code of this response.
    /// </summary>
    public HttpStatusCode Status { get; }

    /// <summary>
    /// The response content as a memory of bytes.
    /// </summary>
    public ReadOnlyMemory<byte> Body => buffer.Memory;

    /// <summary>
    /// The response content as a span of bytes.
    /// </summary>
    public ReadOnlySpan<byte> BodySpan => Body.Span;

    private readonly IMemoryOwner<byte> buffer;

    internal WebResponse(IMemoryOwner<byte> buffer, HttpStatusCode status)
    {
        Status = status;
        this.buffer = buffer;
    }

    /// <summary>
    /// Gets the response content as a string.
    /// </summary>
    /// <returns>The response content as a string.</returns>
    public override string ToString() => Encoding.UTF8.GetString(BodySpan);

    /// <summary>
    /// Gets the response content as a stream.
    /// </summary>
    /// <remarks>
    /// This stream can be disposed at any time as it does not dispose the underlying buffer in <see cref="WebResponse"/>.
    /// However it should be noted that the owning <see cref="WebResponse"/> should not be disposed while the stream is in use.
    /// </remarks>
    /// <returns>The response content as a stream.</returns>
    public Stream AsStream() => Body.AsStream();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            buffer.Dispose();
    }
}
