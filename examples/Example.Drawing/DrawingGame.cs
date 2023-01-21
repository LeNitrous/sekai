// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Graphics.Textures;
using Sekai.Rendering;
using Sekai.Scenes;
using Sekai.Storages;
using Sekai.Windowing;

namespace Example.Drawing;

public class DrawingGame : Game
{
    protected override void Load()
    {
        // Mount the assembly to the virtual file system under the "game" directory.
        Storage.Mount("/game", new AssemblyBackedStorage(typeof(DrawingGame).Assembly, @"Resources"));

        // Create the scene.
        var scene = new Scene
        {
            Nodes = new[]
            {
                // Attaching a 2D transform tells that this node can be drawn in 2D space.
                // Cameras require a 2D transform or else nothing will be rendered to the screen.
                new Node()
                    .AddComponent<Transform2D>()
                    .AddComponent<Camera2D>(),

                // Drawables also require 2D transforms attached to the owning node or else the
                // drawable itself would not be drawn to the scene.
                new Node()
                    .AddComponent<Transform2D>()
                    .AddComponent<TextureProvider>()
                    .AddComponent<TextureDrawingDrawable>()
            }
        };

        Scenes.Add(scene);
    }
}

// "Scriptable"s are basic components which expose a "Load" method.
public class TextureProvider : Scriptable
{
    // The "Resolved" attribute resolves dependencies of any property with a getter
    // and setter. The type defined in the property is the type that will be used in
    // resolving which was defined from building the game. If the dependency is not
    // registered through the service locator, it will throw an exception. Making the
    // property's type nullable makes the dependency an optional which will instead
    // make it so that it returns null if the dependency is not found.
    [Resolved]
    private AssetLoader content { get; set; } = null!;

    // Provide public access to the loaded texture.
    public Texture? Texture;

    // The "Load" method is part of "Scriptable".  It is always called first as soon
    // as the component is loaded into the scene. At this point, we are guaranteed to
    // have a scene and node available so it is referrable to do initialization here
    // rather than doing it in the constructor.
    public override void Load()
    {
        // Load our texture using the asset loader.
        Texture = content.Load<Texture>("/game/image.png");
    }
}

// "Drawable"s are components which provide access to a "Draw" method. As a "Drawable2D",
// It requires a "Transform2D" component attached to the owning node when it is added to
// the scene. 2D drawing operations are performed here.
public class TextureDrawingDrawable : Drawable2D
{
    // Request the surface.
    [Resolved]
    private Surface surface { get; set; } = null!;

    // The "Bind" attribute works similarly to the "Resolved" attribute. However, unlike
    // the "Resolved" attribute which resolves dependencies from the service locator, the
    // "Bind" attribute resolves dependencies through the owning node.
    [Bind]
    private TextureProvider provider { get; set; } = null!;

    // The "Draw" method is part of "Drawable" where the renderer calls this method to
    // perform any drawing that is defined after overriding the method. This method is
    // called after the update step every frame so it is not preferred to call anything
    // too expensive frequently.
    public override void Draw(RenderContext context)
    {
        var texture = provider.Texture!;

        // Get center position.
        float x = (surface.Size.Width / 2) - (texture.Width / 2);
        float y = (surface.Size.Height / 2) - (texture.Height / 2);

        // Draw the texture!
        context.Draw(texture, new Vector2(x, y));
    }
}
