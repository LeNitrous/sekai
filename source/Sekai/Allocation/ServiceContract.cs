// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Allocation;

/// <summary>
/// Describes the contract made by a <see cref="ServiceDescriptor"/>.
/// </summary>
public class ServiceContract : DisposableObject
{
    /// <summary>
    /// The service type.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// The service creation method.
    /// </summary>
    public readonly ServiceKind Kind;

    /// <summary>
    /// The resolved instance.
    /// </summary>
    public readonly object Instance;

    internal ServiceContract(Type type, object instance, ServiceKind kind)
    {
        Type = type;
        Kind = kind;
        Instance = instance;
    }

    protected override void Dispose(bool disposing)
    {
        if (Kind == ServiceKind.Func && Instance is IDisposable disposable)
            disposable.Dispose();
    }
}
