// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sekai.Framework.Extensions;
using Sekai.Framework.Systems;

namespace Sekai.Framework.Entities;

public class SceneManager : GameSystem, IUpdateable
{
    public Scene? Current { get; private set; }
    public IReadOnlyList<EntityProcessor> Processors => processors;
    private readonly List<EntityProcessor> processors = new();
    private readonly List<Entity> currentAliveEntities = new();

    public void Load(Scene scene)
    {
        if (Current != null)
            Unload(Current);

        this.Add(scene);
        Current = scene;
    }

    public void Unload(Scene scene)
    {
        if (Current != scene)
            throw new InvalidOperationException(@"Cannot unload scene as it is not the current.");

        this.Remove(scene);
        Current.Dispose();
    }

    public Task LoadAsync(Scene scene)
    {
        return Task.Run(() => Load(scene));
    }

    public void Add(EntityProcessor processor)
    {
        processors.Add(processor);
    }

    public void Remove(EntityProcessor processor)
    {
        processors.Remove(processor);
    }

    public void Update(double elapsed)
    {
        if (Current is null)
            return;

        getCurrentAliveEntities();

        foreach (var processor in Processors)
        {
            foreach (var entity in currentAliveEntities)
            {
                processor.Update(entity, elapsed);
            }
        }

        clearCurrentAliveEntities();
    }

    private void getCurrentAliveEntities()
    {
        lock (currentAliveEntities)
        {
            if (currentAliveEntities.Count == 0)
                currentAliveEntities.AddRange(Current!.AllAliveEntities);
        }
    }

    private void clearCurrentAliveEntities()
    {
        lock (currentAliveEntities)
        {
            if (currentAliveEntities.Count > 0)
                currentAliveEntities.Clear();
        }
    }

    protected override void OnUnload()
    {
        if (Current != null)
            Unload(Current);
    }
}
