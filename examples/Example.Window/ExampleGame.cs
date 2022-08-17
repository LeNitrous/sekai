// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using Sekai.Engine;
using Sekai.Framework.Annotations;
using Sekai.Framework.Input;
using Sekai.Framework.Logging;

namespace Example.Window;

public class ExampleGame : Game
{
    [Resolved]
    private IInputContext input { get; set; } = null!;

    protected override void Load()
    {
        Logger.OnMessageLogged += new LogListenerConsole();
        var mouse = input.Available.OfType<IMouse>().Single();
        mouse.OnMouseDown += (m, b) => Logger.Log($"Mouse Pressed: {b} @ {m.Position.X},{m.Position.Y}");
    }
}
