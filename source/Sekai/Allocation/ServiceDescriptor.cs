// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;

namespace Sekai.Allocation;

/// <summary>
/// Describes the service made by the <see cref="ServiceCollection"/>.
/// </summary>
[DebuggerDisplay("Type = {Type.FullName, nq}, Kind = {Kind, nq}")]
internal sealed class ServiceDescriptor : IDisposable
{
    /// <summary>
    /// The service type.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// The service creation method.
    /// </summary>
    public readonly ServiceKind Kind;

    private bool isDisposed;
    private object? instance;
    private readonly Func<object>? creator;

    private ServiceDescriptor(Type type, Func<object>? creator, object? instance, ServiceKind kind)
    {
        Type = type;
        Kind = kind;
        this.creator = creator;
        this.instance = instance;
    }

    /// <summary>
    /// Resolves the instance from this descriptor.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The service descriptor failed to resolve the object.</exception>
    public ServiceContract Resolve()
    {
        switch (Kind)
        {
            case ServiceKind.Constant when instance is not null:
                return new(Type, instance, Kind);

            case ServiceKind.Lazy when creator is not null:
                return new(Type, instance ??= creator(), Kind);

            case ServiceKind.Func when creator is not null:
                return new(Type, creator(), Kind);

            default:
                throw new InvalidOperationException("Failed to resolve instance.");
        }
    }

    public static ServiceDescriptor CreateConstant(Type type, object instance) => new(type, null, instance, ServiceKind.Constant);

    public static ServiceDescriptor CreateConstant<T>(T instance) where T : class => new(typeof(T), null, instance, ServiceKind.Constant);

    public static ServiceDescriptor CreateLazy(Type type, Func<object> creator) => new(type, creator, null, ServiceKind.Lazy);

    public static ServiceDescriptor CreateLazy<T>(Func<T> creator) where T : class => new(typeof(T), () => creator(), null, ServiceKind.Lazy);

    public static ServiceDescriptor CreateFunc(Type type, Func<object> creator) => new(type, creator, null, ServiceKind.Func);

    public static ServiceDescriptor CreateFunc<T>(Func<T> creator) where T : class => new(typeof(T), () => creator(), null, ServiceKind.Func);

    public void Dispose()
    {
        if (isDisposed)
            return;

        if (Kind == ServiceKind.Lazy && instance is IDisposable disposable)
            disposable.Dispose();

        isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
