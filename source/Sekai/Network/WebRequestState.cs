// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Network;

/// <summary>
/// The possible <see cref="WebRequest"/> states.
/// </summary>
public enum WebRequestState
{
    /// <summary>
    /// The initial state where it can perform a request.
    /// </summary>
    Initial,

    /// <summary>
    /// The request is currently being performed.
    /// </summary>
    Running,

    /// <summary>
    /// The request has been aborted.
    /// </summary>
    Aborted,

    /// <summary>
    /// The request is fulfilled.
    /// </summary>
    Completed,
}
