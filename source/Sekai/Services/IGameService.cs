// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Services;

public interface IGameService : IDisposable
{
    void Render();

    void Update(double delta);

    void FixedUpdate();
}
