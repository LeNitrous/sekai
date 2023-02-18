// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Allocation;

internal class ServiceCollection : IServiceCollection
{
    private readonly object syncLock = new();
    private readonly List<ServiceDescriptor> services = new();

    public IServiceCollection AddSingleton(Type type)
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateLazy(type, () => Activator.CreateInstance(type)!));

        return this;
    }

    public IServiceCollection AddTransient(Type type)
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateFunc(type, () => Activator.CreateInstance(type)!));

        return this;
    }

    public IServiceCollection AddLazy(Type type, Func<object> creator)
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateLazy(type, creator));

        return this;
    }

    public IServiceCollection AddFunc(Type type, Func<object> creator)
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateFunc(type, creator));

        return this;
    }

    public IServiceCollection AddSingleton<T>()
        where T : class, new()
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateLazy(static () => new T()));

        return this;
    }

    public IServiceCollection AddTransient<T>()
        where T : class, new()
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateFunc(static () => new T()));

        return this;
    }

    public IServiceCollection AddConstant(Type type, object instance)
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateConstant(type, instance));

        return this;
    }

    public IServiceCollection AddConstant<T>(T instance)
        where T : class
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateConstant(instance));

        return this;
    }

    public IServiceCollection AddLazy<T>(Func<T> creator)
        where T : class
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateLazy(creator));

        return this;
    }

    public IServiceCollection AddFunc<T>(Func<T> creator)
        where T : class
    {
        lock (syncLock)
            services.Add(ServiceDescriptor.CreateFunc(creator));

        return this;
    }

    public IServiceCollection RemoveAll()
    {
        lock (syncLock)
            services.Clear();

        return this;
    }

    public IServiceCollection RemoveAll<T>()
        where T : class
    {
        lock (syncLock)
            services.RemoveAll(r => r.Type.IsAssignableTo(typeof(T)));

        return this;
    }

    public ServiceLocator CreateLocator() => new(new List<ServiceDescriptor>(services));
}
