// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading.Tasks;

namespace Sekai.Processors;

internal sealed class ScriptableProcessor : Processor<Scriptable>
{
    protected override void OnComponentAttach(Scriptable component)
    {
        component.Load();
        Task.Factory.StartNew(component.LoadAsync, default, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
    }

    protected override void OnComponentDetach(Scriptable component)
    {
        component.Unload();
        Task.Factory.StartNew(component.UnloadAsync, default, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
