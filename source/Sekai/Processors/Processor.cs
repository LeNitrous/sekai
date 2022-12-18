// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Scenes;

namespace Sekai.Processors;

public abstract class Processor : ActivateableObject
{
    public Scene? Scene { get; private set; }

    public void Update(double delta)
    {
        if (Enabled)
            Process(delta);
    }

    protected abstract void Process(double delta);
    internal abstract void Attach(IProcessorAttachable attachable);
    internal abstract void Detach(IProcessorAttachable attachable);

    internal void Attach(Scene scene)
    {
        if (IsAttached)
            return;

        if (scene is null)
            throw new ArgumentNullException(nameof(scene));

        Scene = scene;

        Attach();
    }

    internal void Detach(Scene scene)
    {
        if (!IsAttached)
            return;

        if (Scene != scene)
            throw new InvalidOperationException(@"Cannot detach from a scene not owning this processor.");

        Scene = null;

        Detach();
    }
}

public abstract class Processor<T> : Processor
    where T : IProcessorAttachable
{
    private readonly List<T> attached = new();

    protected virtual void Process(double delta, T attachable)
    {
    }

    protected virtual void Attach(T attachable)
    {
    }

    protected virtual void Detach(T attachable)
    {
    }

    protected override void Process(double delta)
    {
        var attached = this.attached.ToArray();

        foreach (var attachable in attached)
            Process(delta, attachable);
    }

    internal sealed override void Attach(IProcessorAttachable attachable)
    {
        if (attachable is T attach && !attached.Contains(attach))
        {
            attached.Add(attach);
            Attach(attach);
        }
    }

    internal sealed override void Detach(IProcessorAttachable attachable)
    {
        if (attachable is T attach && attached.Remove(attach))
            Detach(attach);
    }
}
