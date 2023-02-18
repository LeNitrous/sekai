// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Allocation;

/// <summary>
/// An interface allowing registration of services.
/// </summary>
public interface IServiceCollection
{
    /// <summary>
    /// Adds a singleton service.
    /// </summary>
    /// <param name="type">The object type to instantiate.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// Singleton services instantiate the object lazily and subsequent requests use the same object.
    /// </remarks>
    IServiceCollection AddSingleton(Type type);

    /// <summary>
    /// Adds a transient service.
    /// </summary>
    /// <param name="type">The object type to instantiate.</param>
    /// <returns>The service collection.</returns>
    /// <renarks>
    /// Transient services instantiate a new instance of an object every request.
    /// </renarks>
    IServiceCollection AddTransient(Type type);

    /// <summary>
    /// Adds a constant service.
    /// </summary>
    /// <param name="type">The object type.</param>
    /// <param name="instance">The instance to be cached.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// Constant services cache the object immediately without lazy loading.
    /// </remarks>
    IServiceCollection AddConstant(Type type, object instance);

    /// <summary>
    /// Adds a lazily loaded object which is created using the <paramref name="creator"/>.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="creator">The function used to create the instance.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddLazy(Type type, Func<object> creator);

    /// <summary>
    /// Adds a transient loaded object which is created using the <paramref name="creator"/>.
    /// </summary>
    /// <param name="type">The object type.</param>
    /// <param name="creator">The function used to create the instance.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddFunc(Type type, Func<object> creator);

    /// <inheritdoc cref="AddSingleton(Type)"/>
    /// <typeparam name="T">The type of object to instantiate.</typeparam>
    IServiceCollection AddSingleton<T>() where T : class, new();

    /// <inheritdoc cref="AddTransient(Type)"/>
    /// <typeparam name="T">The type of object to instantiate.</typeparam>
    IServiceCollection AddTransient<T>() where T : class, new();

    /// <inheritdoc cref="AddConstant(Type, object)"/>
    /// <typeparam name="T">The type of object to instantiate.</typeparam>
    IServiceCollection AddConstant<T>(T instance) where T : class;

    /// <inheritdoc cref="AddLazy(Type, Func{object})"/>
    /// <typeparam name="T">The type of object to instantiate.</typeparam>
    IServiceCollection AddLazy<T>(Func<T> creator) where T : class;

    /// <inheritdoc cref="AddFunc(Type, Func{object)"/>
    /// <typeparam name="T">The type of object to instantiate.</typeparam>
    IServiceCollection AddFunc<T>(Func<T> creator) where T : class;

    /// <summary>
    /// Removes all added services in this collection.
    /// </summary>
    /// <returns>The service collection.</returns>
    IServiceCollection RemoveAll();

    /// <summary>
    /// Removes all services of the given type in this collection.
    /// </summary>
    /// <typeparam name="T">The type of service to remove.</typeparam>
    /// <returns>The service collection.</returns>
    IServiceCollection RemoveAll<T>() where T : class;
}
