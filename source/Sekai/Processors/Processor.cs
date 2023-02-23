// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.CompilerServices;
using Sekai.Scenes;

namespace Sekai.Processors;

public abstract class Processor : ServiceableObject
{
    public virtual int Priority => 0;

    public virtual void OnComponentAttach(Component component)
    {
    }

    public virtual void OnComponentDetach(Component component)
    {
    }

    public virtual void Update(Component component)
    {
    }
}

public abstract class Processor<T> : Processor
    where T : Component
{
    public sealed override void Update(Component component)
        => Update(Unsafe.As<T>(component));

    public sealed override void OnComponentAttach(Component component)
        => OnComponentAttach(Unsafe.As<T>(component));

    public sealed override void OnComponentDetach(Component component)
        => OnComponentDetach(Unsafe.As<T>(component));

    protected virtual void OnComponentAttach(T component)
    {
    }

    protected virtual void OnComponentDetach(T component)
    {
    }

    protected virtual void Update(T component)
    {
    }

    protected virtual void Render(T component)
    {
    }
}
