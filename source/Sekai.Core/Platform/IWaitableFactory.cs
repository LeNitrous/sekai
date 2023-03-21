// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// A factory for creating waitable objects.
/// </summary>
public interface IWaitableFactory
{
    /// <summary>
    /// Creates a new waitable object.
    /// </summary>
    /// <returns>The waitable object.</returns>
    IWaitable CreateWaitable();
}
