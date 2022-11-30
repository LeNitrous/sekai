// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai;

public abstract class Scriptable : Component
{
    public bool HasStarted { get; private set; }

    protected abstract void Start();

    internal void Load()
    {
        if (HasStarted)
            return;

        Start();

        HasStarted = true;
    }

    protected override void OnAttach() => Scene?.Get<ScriptableProcessor>().Enqueue(this);
    protected override void OnDetach() => HasStarted = false;
}
