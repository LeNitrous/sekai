// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// A helper class that exposes methods related to .NET Hot Reload.
/// </summary>
public static class HotReload
{
    /// <summary>
    /// Invoked when the metadata has updated.
    /// </summary>
    public static event Action<Type[]?>? OnUpdate;

    /// <summary>
    /// Invoked when the cache has been cleared.
    /// </summary>
    public static event Action<Type[]?>? OnClear;

#pragma warning disable IDE1006 // Required by MetadataUpdateHandler
#pragma warning disable IDE0051 // Invoked via MetadataUpdateHandler

    private static void UpdateApplication(Type[]? updated) => OnUpdate?.Invoke(updated);

    private static void ClearCache(Type[]? updated) => OnClear?.Invoke(updated);

#pragma warning restore IDE1006
#pragma warning restore IDE0051

}
