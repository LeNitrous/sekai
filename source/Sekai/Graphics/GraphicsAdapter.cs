// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Graphics;

/// <summary>
/// Describes the <see cref="GraphicsDevice"/> and its capabilities.
/// </summary>
public abstract class GraphicsAdapter
{
    /// <summary>
    /// The adapter's name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The adapter's vendor.
    /// </summary>
    public abstract string Vendor { get; }

    /// <summary>
    /// The adapter's version.
    /// </summary>
    public abstract Version Version { get; }

    /// <summary>
    /// Gets whether a given graphics device feature is supported or not.
    /// </summary>
    /// <param name="feature">The feature to check.</param>
    /// <returns><see langword="true"/> if the feature is supported. Otherwise, <see langword="false"/>.</returns>
    public abstract bool IsFeatureSupported(GraphicsDeviceFeatures feature);

    /// <summary>
    /// Gets an enumeration of all supported graphics device features.
    /// </summary>
    /// <returns>An enumeration of all upported graphics device features.</returns>
    public abstract IEnumerable<GraphicsDeviceFeatures> GetDeviceFeatures();
}
