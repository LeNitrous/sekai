// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Processors;

public interface IUpdateable : IProcessor
{
    void Update(double delta);
}
