// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Scenes;

public class ComponentEventArgs : EventArgs
{
    public readonly Component Component;

    public ComponentEventArgs(Component component)
    {
        Component = component;
    }
}
