// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Services;

/// <summary>
/// A version of <see cref="IGameService"/> that draws on the screen.
/// </summary>
public interface IDrawingGameService : IGameService
{
    /// <summary>
    /// Renders the service. This only applies to services that requires to draw to the screen.
    /// </summary>
    void Render();
}
