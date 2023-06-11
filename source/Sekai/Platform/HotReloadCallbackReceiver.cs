// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform;

internal static class HotReloadCallbackReceiver
{
    public static event Action<Type[]>? OnUpdate;
    public static void UpdateApplication(Type[] updated) => OnUpdate?.Invoke(updated);
}
