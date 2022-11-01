// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Logging;
using Sekai.Framework.Scenes;

namespace Example.Window;

public class ExampleGame : Game
{
    protected override void Load()
    {
        var scene = new Scene
        {
            Entities = new[]
            {
                new Entity
                {
                    Components = new Component[]
                    {
                        new Camera { Width = 1280, Height = 720, NearClipPlane = -1f, FarClipPlane = 1f, Projection = CameraProjectionMode.Orthographic },
                        new HelloComponent(),
                        new Transform(),
                    }
                },
                new Entity
                {
                    Components = new Component[]
                    {
                        new Transform(),
                        new Rectangle(),
                    }
                }
            }
        };

        Scenes.Push(scene);
    }
}

public class HelloComponent : Component
{
    public override void Start()
    {
        Logger.Log("Hello World!");
    }
}

public class Rectangle : Drawable
{
    public override void Draw()
    {
    }
}
