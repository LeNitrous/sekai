// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;

namespace Hello;

public class HelloGame : Game
{
    public override void Load()
    {
        Scenes.Add(new() { Root = { Components = new[] { new HelloScript() } } });
    }
}
