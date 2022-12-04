// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Rendering;

/// <summary>
/// Determines how <see cref="Drawable"/>s are sorted when being rendered.
/// </summary>
public enum SortMode
{
    /// <summary>
    /// Front-to-back mode. Objects are rendered from the front of the camera to the back of the camera.
    /// </summary>
    FrontToBack,

    /// <summary>
    /// Back-to-front mode. Objects are rendered from the back of the camera to the front of the camera.
    /// </summary>
    BackToFront,
}
