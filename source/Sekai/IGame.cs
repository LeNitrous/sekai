// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

internal interface IGame
{
    void Run();
    void Exit();
    void Render();
    void Update(double delta);
    void FixedUpdate();
}
