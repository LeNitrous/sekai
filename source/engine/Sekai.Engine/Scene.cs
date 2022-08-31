// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using Sekai.Engine.Processors;
using Sekai.Engine.Rendering;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

[Cached]
public class Scene : EntityCollection, IUpdateable, IRenderable
{
    /// <summary>
    /// Gets or sets the name for this scene.
    /// </summary>
    public string Name { get; set; } = "Scene";

    /// <summary>
    /// The scene camera.
    /// </summary>
    public Camera Camera { get; set; } = null!;

    [Cached]
    private readonly SystemCollection<SceneSystem> systems = new();

    public Scene()
    {
        AddInternal(systems);
        systems.Register<RenderContext>();
        systems.Register<MeshProcessor>();
        systems.Register<CameraProcessor>();
        systems.Register<BehaviorProcessor>();
        systems.Register<TransformProcessor>();
    }

    public void Render()
    {
        foreach (var system in systems.Where(s => s.IsAlive).OfType<IRenderable>())
            system.Render();
    }

    public void Update(double elapsed)
    {
        foreach (var system in systems.Where(s => s.IsAlive).OfType<IUpdateable>())
            system.Update(elapsed);
    }

    /// <summary>
    /// Notifies processors that the entity has been added/removed, activated/deactivated or added/removed components
    /// </summary>
    internal void OnEntityUpdate(Entity entity)
    {
        foreach (var processor in systems.OfType<Processor>())
            processor.OnEntityUpdate(entity);
    }
}
