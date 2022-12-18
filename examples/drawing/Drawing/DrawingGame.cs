// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;
using Sekai.Mathematics;
using Sekai.Rendering;
using Sekai.Scenes;

namespace Drawing;

public class DrawingGame : Game
{
    public override void Load()
    {
        Scenes.Add
        (
            new Scene2D
            {
                Root =
                {
                    Components = new Component[]
                    {
                        new Camera2D(),
                        new QuadDrawingScript(),
                    }
                }
            }
        );
    }

    private class QuadDrawingScript : Drawable2D
    {
        public override void Start()
        {
        }

        public override void Draw(Renderer2D renderer)
        {
            for (int x = 0; x < 25; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    renderer.Draw(new Rectangle(x * 15, y * 15, 10, 10), new Color4(0.4f));
                }
            }
        }
    }
}
