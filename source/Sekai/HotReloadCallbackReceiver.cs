// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

internal static class HotReloadCallbackReceiver
{
    public static event Action<Type[]?>? OnUpdate;

#pragma warning disable IDE1006 // Required by MetadataUpdateHandler
#pragma warning disable IDE0051 // Invoked via MetadataUpdateHandler

    private static void UpdateApplication(Type[]? updated) => OnUpdate?.Invoke(updated);

#pragma warning restore IDE1006
#pragma warning restore IDE0051

}
