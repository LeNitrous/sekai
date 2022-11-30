// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Allocation;

/// <summary>
/// A container for registering or resolving cached services.
/// </summary>
public class Services : FrameworkObject, IServices
{
    public static readonly Services Current = new();

    private readonly Dictionary<Type, Func<object>> cache;

    private Services()
        : this(new Dictionary<Type, Func<object>>())
    {
    }

    private Services(Services other)
        : this(new Dictionary<Type, Func<object>>(other.cache))
    {
    }

    private Services(Dictionary<Type, Func<object>> cache)
    {
        this.cache = cache;
    }

    public void Register(Type type, Func<object> instance)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Services));

        if (cache.ContainsKey(type))
            throw new InvalidOperationException();

        cache.Add(type, instance);
    }

    public void Register(Type type, object instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        if (!instance.GetType().IsAssignableTo(type))
            throw new InvalidCastException();

        Register(type, () => instance);
    }

    public void Register<T>(Func<T> instance)
    {
        Register(typeof(T), () => instance()!);
    }

    public void Register<T>(T instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        Register(typeof(T), instance);
    }

    public void Register<T>()
        where T : new()
    {
        Register(Activator.CreateInstance<T>());
    }

    public object? Resolve(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Services));

        if (cache.TryGetValue(type, out var result))
        {
            object instance = result();

            if (!instance.GetType().IsAssignableTo(type))
                throw new InvalidCastException();

            return instance;
        }

        if (required)
            throw new KeyNotFoundException();

        return null;
    }

    public T? Resolve<T>([DoesNotReturnIf(true)] bool required = true)
    {
        object? result = Resolve(typeof(T), required);
        return result is null ? default : (T)result;
    }

    protected override void Destroy() => cache.Clear();

    internal Services CreateScoped() => new(this);
}
