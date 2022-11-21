// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Services;
/// <summary>
/// The base interface for all game services
/// </summary>
public interface IGameService : IDisposable
{
    /// <summary>
    /// Renders the service. This only applies to services that requires to draw to the screen.
    /// </summary>
    /// FIXME: Please don't make this required because not all services requires to render to the screen.
    void Render();

    /// <summary>
    /// Updates the service with a delta attached.
    /// </summary>
    /// <param name="delta"></param>
    void Update(double delta);

    /// <summary>
    /// A version of the <see cref="Update(double)"/> but works on a fixed time step.
    /// </summary>
    void FixedUpdate();
}
