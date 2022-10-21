// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Engine.Effects;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public class RenderContext : SceneSystem, IRenderable
{
    private readonly List<CameraInfo> cameras = new();
    private readonly List<RenderObject> renderables = new();
    private readonly List<Type> rendererKeys = new();
    private readonly List<string> stages = new();
    private readonly Dictionary<Type, RenderFeature> renderers = new();
    private readonly Dictionary<string, RenderParameter> parameters = new();
    private readonly IGraphicsDevice device = Game.Resolve<IGraphicsDevice>();

    public RenderContext()
    {
        AddRenderStage("Default");
        AddParameter<Matrix4x4>("View");
        AddParameter<Matrix4x4>("Projection");
        AddRenderFeature<MeshRenderObject, MeshRenderPipeline>();
    }

    public void Render()
    {
        for (int x = 0; x < cameras.Count; x++)
        {
            var camera = cameras[x];

            UpdateParameter("View", camera.ViewMatrix);
            UpdateParameter("Projection", camera.ProjMatrix);

            for (int i = 0; i < rendererKeys.Count; i++)
            {
                var key = rendererKeys[i];
                var renderer = renderers[key];

                for (int j = 0; j < renderables.Count; j++)
                {
                    var renderable = renderables[j];

                    if (renderable.GetType() != key)
                        continue;

                    for (int k = 0; k < stages.Count; k++)
                    {
                        string stage = stages[k];
                        renderer.Render(this, stage, renderable);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds a render feature for this render context.
    /// </summary>
    public void AddRenderFeature<TObject, TPipeline>()
        where TObject : RenderObject, new()
        where TPipeline : RenderFeature<TObject>, new()
    {
        var key = typeof(TObject);

        if (renderers.ContainsKey(key))
            return;

        rendererKeys.Add(key);
        renderers.Add(key, Activator.CreateInstance<TPipeline>());
    }

    /// <summary>
    /// Adds a render stage for this render context.
    /// </summary>
    public void AddRenderStage(string name)
    {
        if (stages.Contains(name))
            return;

        stages.Add(name);
    }

    /// <summary>
    /// Creates a material owned by this render context.
    /// </summary>
    public Material CreateMaterial(Effect effect)
    {
        return new Material(this, effect);
    }

    /// <summary>
    /// Adds a renderable object to the scene.
    /// </summary>
    public void Add(RenderObject renderable)
    {
        if (renderables.Contains(renderable))
            return;

        renderables.Add(renderable);
    }

    /// <summary>
    /// Removes a renderable object from the scene.
    /// </summary>
    public void Remove(RenderObject renderable)
    {
        renderables.Remove(renderable);
    }

    /// <summary>
    /// Adds a camera to the scene.
    /// </summary>
    internal void AddCamera(CameraInfo camera)
    {
        cameras.Add(camera);
    }

    /// <summary>
    /// Removes a camera from the scene.
    /// </summary>
    internal void RemoveCamera(CameraInfo camera)
    {
        cameras.Remove(camera);
    }

    /// <summary>
    /// Retrieves the device resource of a given global parameter.
    /// </summary>
    internal bool GetParameter(string name, [NotNullWhen(true)] out IBindableResource? resource)
    {
        if (!parameters.ContainsKey(name))
        {
            resource = null;
            return false;
        }

        resource = parameters[name].Resource;
        return true;
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter<T>(string name, bool asUniform = true)
        where T : struct, IEquatable<T>
    {
        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        int size = Marshal.SizeOf<T>();
        var desc = new BufferDescription((uint)size, asUniform ? BufferUsage.Uniform | BufferUsage.Dynamic : BufferUsage.StructuredReadOnly);
        parameters.Add(name, new RenderParameter<T>(name, device, device.Factory.CreateBuffer(ref desc)));
    }

    /// <summary>
    /// Updates the value of a given global parameter.
    /// </summary>
    protected void UpdateParameter<T>(string name, T value)
        where T : struct, IEquatable<T>
    {
        name = $"g_{name}";

        if (parameters.TryGetValue(name, out var resource))
        {
            if (resource is not RenderParameter<T> param)
                throw new InvalidOperationException("The parameter is read-only and cannot be updated.");

            param.Value = value;
        }
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter(string name, ITexture texture)
    {
        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        parameters.Add(name, new RenderParameter(name, texture));
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter(string name, ISampler sampler)
    {
        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        parameters.Add(name, new RenderParameter(name, sampler));
    }

    protected override void Destroy()
    {
        foreach ((string name, RenderParameter param) in parameters)
            param.Dispose();
    }
}
