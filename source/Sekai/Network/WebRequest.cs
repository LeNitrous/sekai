// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sekai.Network;

/// <summary>
/// An object that performs web requests.
/// </summary>
public class WebRequest : DisposableObject
{
    /// <summary>
    /// The URL defined in the constructor.
    /// </summary>
    public readonly Uri Url;

    /// <summary>
    /// The request method.
    /// </summary>
    public HttpMethod Method = HttpMethod.Get;

    /// <summary>
    /// The time span on how long it will take before timing out.
    /// </summary>
    public TimeSpan Timeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Whether to allow insecure requests or not.
    /// </summary>
    public bool AllowInsecureRequests;

    /// <summary>
    /// Invoked when data has been sent.
    /// </summary>
    public event Action<long, long>? OnDataSent;

    /// <summary>
    /// Invoked when data has been received.
    /// </summary>
    public event Action<long, long>? OnDataReceived;

    /// <summary>
    /// The state of this web request.
    /// </summary>
    public WebRequestState State { get; private set; }

    private CancellationTokenSource? tokenAbort;
    private CancellationTokenSource? tokenTimeout;
    private IMemoryOwner<byte>? content;
    private Dictionary<string, string>? headers;
    private Dictionary<string, string>? parametersForm;
    private Dictionary<string, string>? parametersQuery;
    private Dictionary<string, IMemoryOwner<byte>>? files;

    private static readonly HttpClient client = new
    (
        RuntimeInfo.OS == RuntimeInfo.Platform.Android
            ? new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultCredentials,
                AutomaticDecompression = DecompressionMethods.All
            }
            : new SocketsHttpHandler
            {
                Credentials = CredentialCache.DefaultCredentials,
                AutomaticDecompression = DecompressionMethods.All,
            }
    )
    {
        Timeout = System.Threading.Timeout.InfiniteTimeSpan
    };

    public WebRequest(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        Url = url;
    }

    public WebRequest(string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            throw new ArgumentException($@"""{url}"" is not a valid url.", nameof(url));

        Url = uri;
    }

    /// <summary>
    /// Sets the content to be sent.
    /// </summary>
    /// <param name="data">The content to be sent.</param>
    public WebRequest SetContent(string data)
    {
        Span<byte> buffer = stackalloc byte[data.Length];
        int written = Encoding.UTF8.GetBytes(data, buffer);
        return SetContent(buffer);
    }

    /// <inheritdoc cref="SetContent(string)"/>
    public WebRequest SetContent(Stream data)
    {
        Span<byte> buffer = stackalloc byte[(int)data.Length];
        data.Read(buffer);
        return SetContent(buffer);
    }

    /// <inheritdoc cref="SetContent(string)"/>
    public WebRequest SetContent(byte[] data)
        => SetContent((ReadOnlySpan<byte>)data);

    /// <inheritdoc cref="SetContent(string)"/>
    public WebRequest SetContent(ReadOnlySpan<byte> data)
    {
        if (State != WebRequestState.Initial)
            throw new InvalidOperationException(@"Cannot set content on a non-initial state request.");

        content?.Dispose();
        content = MemoryPool<byte>.Shared.Rent(data.Length);
        data.CopyTo(content.Memory.Span);
        return this;
    }

    /// <summary>
    /// Adds a file to be sent in this request.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="data">The file data.</param>
    public WebRequest AddFile(string name, Stream data)
    {
        Span<byte> buffer = stackalloc byte[(int)data.Length];
        data.Read(buffer);
        return AddFile(name, buffer);
    }

    /// <inheritdoc cref="AddFile(string, Stream)"/>
    public WebRequest AddFile(string name, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        return AddFile(name, (ReadOnlySpan<byte>)data);
    }

    /// <inheritdoc cref="AddFile(string, Stream)"/>
    public WebRequest AddFile(string name, ReadOnlySpan<byte> data)
    {
        if (State != WebRequestState.Initial)
            throw new InvalidOperationException(@"Cannot add files on a non-initial state request.");

        ArgumentNullException.ThrowIfNull(name, nameof(name));

        var owner = MemoryPool<byte>.Shared.Rent(data.Length);
        data.CopyTo(owner.Memory.Span);

        files ??= new();
        files.Add(name, owner);

        return this;
    }

    /// <summary>
    /// Adds a request header.
    /// </summary>
    /// <param name="name">The header key.</param>
    /// <param name="value">The header value.</param>
    public WebRequest AddHeader(string name, string value)
    {
        if (State != WebRequestState.Initial)
            throw new InvalidOperationException(@"Cannot add headers on a non-initial state request.");

        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        headers ??= new();
        headers.Add(name, value);

        return this;
    }

    /// <inheritdoc cref="AddParameter(string, string, WebRequestParameterType)"/>
    public WebRequest AddParameter(string name, string value)
        => AddParameter(name, value, Method.GetRequestParameterType());

    /// <summary>
    /// Adds a request parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value/</param>
    /// <param name="type">The parameter type.</param>
    public WebRequest AddParameter(string name, string value, WebRequestParameterType type)
    {
        if (State != WebRequestState.Initial)
            throw new InvalidOperationException(@"Cannot add parameters on a non-initial state request.");

        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        switch (type)
        {
            case WebRequestParameterType.Query:
                {
                    parametersQuery ??= new();
                    parametersQuery.Add(name, value);
                    break;
                }

            case WebRequestParameterType.Form:
                {
                    if (!Method.SupportsRequestBody())
                        throw new ArgumentException("Cannot add form parameter to a request which has no form body.", nameof(type));

                    parametersForm ??= new();
                    parametersForm.Add(name, value);
                    break;
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }

        return this;
    }

    /// <summary>
    /// Aborts this web request.
    /// </summary>
    public void Abort()
    {
        if (State != WebRequestState.Running)
            return;

        State = WebRequestState.Aborted;

        tokenAbort?.Cancel();
    }

    /// <summary>
    /// Performs this request.
    /// </summary>
    public WebResponse Perform()
    {
        return PerformAsync().Result;
    }

    /// <summary>
    /// Performs this request asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The response content.</returns>
    public async Task<WebResponse> PerformAsync(CancellationToken token = default)
    {
        State = WebRequestState.Running;
        string url = Url.OriginalString;

        if (!AllowInsecureRequests && Url.Scheme != @"https")
            url = @"https://" + url.Replace(@"https://", string.Empty);

        using (tokenAbort ??= new CancellationTokenSource())
        using (tokenTimeout = new CancellationTokenSource())
        using (var tokens = CancellationTokenSource.CreateLinkedTokenSource(tokenAbort.Token, tokenTimeout.Token, token))
        using (var request = new HttpRequestMessage(Method, url))
        {
            try
            {
                if (parametersQuery is not null)
                {
                    string query = string.Join('&', parametersQuery.Select(p => $@"{p.Key}={p.Value}"));
                    url += string.IsNullOrEmpty(Url.Query) ? '?' + query : query;
                }

                if (headers is not null)
                {
                    foreach (var h in headers)
                        request.Headers.Add(h.Key, h.Value);
                }

                HttpContent? httpContent = null;

                if (content is not null)
                {
                    if (parametersForm is not null)
                        throw new InvalidOperationException("Request cannot contain raw data in conjunction with form parameters.");

                    if (files is not null)
                        throw new InvalidOperationException("Request cannot contain raw data in conjunction with file upload.");

                    httpContent = new ReadOnlyMemoryContent(content.Memory);
                }

                if (parametersForm is not null || files is not null)
                {
                    var form = new MultipartFormDataContent();

                    if (parametersForm is not null)
                    {
                        foreach (var p in parametersForm)
                            form.Add(new StringContent(p.Value), p.Key);
                    }

                    if (files is not null)
                    {
                        foreach (var f in files)
                            form.Add(new ReadOnlyMemoryContent(f.Value.Memory), f.Key, f.Key);
                    }

                    httpContent = form;
                }

                if (httpContent is not null)
                {
                    var tracked = new ProgressTrackingContent(httpContent);

                    tracked.OnProgress += (current, total) =>
                    {
                        tokenTimeout.CancelAfter(Timeout);
                        OnDataSent?.Invoke(current, total);
                    };

                    request.Content = tracked;
                }

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokens.Token).ConfigureAwait(false);

                using (var stream = await response.Content.ReadAsStreamAsync(tokens.Token))
                using (var memory = new MemoryStream())
                using (var buffer = MemoryPool<byte>.Shared.Rent(8192))
                {
                    int current = 0;

                    while (true)
                    {
                        tokens.Token.ThrowIfCancellationRequested();

                        int read = await stream.ReadAsync(buffer.Memory, tokens.Token);

                        if (read > 0)
                        {
                            await memory.WriteAsync(buffer.Memory[..read], tokens.Token);
                            tokenTimeout.CancelAfter(Timeout);

                            current += read;
                            OnDataReceived?.Invoke(current, response.Content.Headers.ContentLength ?? current);
                        }
                        else
                        {
                            State = WebRequestState.Completed;
                            break;
                        }
                    }

                    var received = MemoryPool<byte>.Shared.Rent(current);

                    memory.Position = 0;
                    await memory.ReadAsync(received.Memory, tokens.Token);

                    return new WebResponse(received, response.StatusCode);
                }
            }
            catch (Exception) when (tokenAbort.IsCancellationRequested || token.IsCancellationRequested)
            {
                Abort();
                throw;
            }
            catch
            {
                throw;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        if (files is not null)
        {
            foreach (var fileBuffer in files.Values)
                fileBuffer.Dispose();
        }

        content?.Dispose();
    }

    private sealed class ProgressTrackingContent : HttpContent
    {
        public event Action<long, long>? OnProgress;

        private readonly HttpContent content;

        public ProgressTrackingContent(HttpContent content)
        {
            this.content = content;

            foreach (var h in content.Headers)
                Headers.Add(h.Key, h.Value);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            using var buffer = MemoryPool<byte>.Shared.Rent(8192);

            var readable = await content.ReadAsStreamAsync();

            int read;
            int uploaded = 0;
            bool hasLength = TryComputeLength(out long length);

            while ((read = await readable.ReadAsync(buffer.Memory)) > 0)
            {
                await stream.WriteAsync(buffer.Memory);
                await stream.FlushAsync();

                uploaded += read;

                if (hasLength)
                    OnProgress?.Invoke(uploaded, length);
            }

            await stream.FlushAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = Headers.ContentLength.GetValueOrDefault();
            return Headers.ContentLength.HasValue;
        }
    }
}

public static class HttpMethodExtensions
{
    public static bool SupportsRequestBody(this HttpMethod method)
        => method == HttpMethod.Post
            || method == HttpMethod.Put
            || method == HttpMethod.Delete
            || method == HttpMethod.Patch;

    public static WebRequestParameterType GetRequestParameterType(this HttpMethod method)
        => SupportsRequestBody(method) ? WebRequestParameterType.Form : WebRequestParameterType.Query;
}
