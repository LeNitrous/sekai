// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Processors;

internal sealed class ScriptableProcessor : Processor<Scriptable>
{
    protected override void OnComponentAttach(Scriptable component) => component.Load();
    protected override void OnComponentDetach(Scriptable component) => component.Unload();
}
