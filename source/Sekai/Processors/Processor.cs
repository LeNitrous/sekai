// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Scenes;

namespace Sekai.Processors;

public abstract class Processor : ActivateableObject, IProcessor
{
    public Scene? Scene { get; private set; }

    internal void Attach(Scene scene)
    {
        if (IsAttached)
            return;

        if (scene is null)
            throw new InvalidOperationException();

        Scene = scene;

        Attach();
    }

    internal void Detach(Scene scene)
    {
        if (!IsAttached)
            return;

        if (Scene != scene)
            throw new InvalidOperationException();

        Scene = null;

        Detach();
    }
}
