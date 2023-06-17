// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Framework.Platform.Windowing;

/// <summary>
/// Information about a video mode for an <see cref="IMonitor"/>.
/// </summary>
/// <param name="Resolution">The video mode resolution.</param>
/// <param name="RefreshRate">The video mode refresh rate.</param>
public readonly record struct VideoMode(Size Resolution, int RefreshRate);
