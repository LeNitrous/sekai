// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework;

internal interface IGame
{
    void Run();
    void Exit();
    void Render();
    void Update(double delta);
    void FixedUpdate();
}
