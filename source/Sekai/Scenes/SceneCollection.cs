// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Graphics;
using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A collection of scenes.
/// </summary>
public partial class SceneCollection : ServiceableObject, ICollection<Scene>, IReadOnlyList<Scene>
{
    [Resolved]
    private GraphicsContext graphics { get; set; } = null!;

    [Resolved]
    private AssetLoader assets { get; set; } = null!;

    public Scene this[int index] => scenes[index];
    public int Count => scenes.Count;
    internal Renderer Renderer { get; }
    internal IEnumerable<Processor> Processors => processors;

    private readonly object syncLock = new();
    private readonly List<Scene> scenes = new();
    private readonly List<Processor> processors = new();
    private readonly Dictionary<Type, Processor> processorMapping = new();
    private readonly Dictionary<Type, ProcessorAttribute[]> processorsForComponent = new();

    internal SceneCollection()
    {
        Renderer = new(graphics, assets);
    }

    internal void Update()
    {
        lock (syncLock)
        {
            foreach (var scene in scenes)
            {
                Renderer.Begin(scene.Kind);
                scene.Update(this);
                Renderer.End();
            }
        }
    }

    internal void Render()
    {
        lock (syncLock)
        {
            foreach (var scene in scenes)
                Renderer.Render();
        }
    }

    /// <summary>
    /// Adds a scene to this collection.
    /// </summary>
    public void Add(Scene item)
    {
        lock (syncLock)
            scenes.Add(item);
    }

    /// <summary>
    /// Removes all scenes in this collection.
    /// </summary>
    public void Clear()
    {
        lock (syncLock)
            scenes.Clear();
    }

    /// <summary>
    /// Returns whether a given scene is in this collection.
    /// </summary>
    public bool Contains(Scene item)
    {
        lock (syncLock)
            return scenes.Contains(item);
    }

    /// <summary>
    /// Copies a range of scenes to the array at a given index.
    /// </summary>
    public void CopyTo(Scene[] array, int arrayIndex)
    {
        lock (syncLock)
            scenes.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes a scene from this collection.
    /// </summary>
    public bool Remove(Scene item)
    {
        lock (syncLock)
            return scenes.Remove(item);
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        lock (syncLock)
            return scenes.GetEnumerator();
    }

    internal IEnumerable<Processor> GetProcessors(Component component)
    {
        var type = component.GetType();

        if (!processorsForComponent.TryGetValue(type, out var attribs))
        {
            attribs = type.GetCustomAttributes<ProcessorAttribute>(true).ToArray();
            processorsForComponent.Add(type, attribs);
        }

        foreach (var attrib in attribs)
        {
            if (!processorMapping.TryGetValue(attrib.Type, out var processor))
            {
                processor = attrib.CreateInstance();
                processorMapping.Add(attrib.Type, processor);

                processors.Add(processor);
                processors.Sort(processorComparer);
            }

            yield return processor;
        }
    }

    bool ICollection<Scene>.IsReadOnly => false;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static readonly IComparer<Processor> processorComparer = Comparer<Processor>.Create((a, b) => a.Priority.CompareTo(b.Priority));
}
