// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Used by objects that can be collected by the <see cref="Renderer"/>.
/// </summary>
internal interface IRenderObject
{
    /// <summary>
    /// The kind of scene this <see cref="IRenderObject"/> can be collected to.
    /// </summary>
    RenderKind Kind { get; }
}
