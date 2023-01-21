// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Scenes;

public class NodeEventArgs : EventArgs
{
    public readonly Node Node;

    public NodeEventArgs(Node node)
    {
        Node = node;
    }
}
