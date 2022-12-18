// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sekai.Scenes;

namespace Sekai.Processors;

/// <summary>
/// A collection of processors that manage a <see cref="Component"/>.
/// </summary>
public sealed class ProcessorCollection : FrameworkObject, IReadOnlyCollection<Processor>
{
    /// <summary>
    /// The number of processors currently registered to this collection.
    /// </summary>
    public int Count => processors.Count;

    private Scene? scene;
    private readonly List<Processor> processors = new();
    private readonly Dictionary<Type, Type[]> attachableMap = new();
    private readonly Dictionary<Type, Processor> processorMap = new();

    /// <summary>
    /// Registers a processor to this collection.
    /// </summary>
    /// <typeparam name="T">The processor type to be registered.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when attempting to register an already registered processor.</exception>
    public void Register<T>()
        where T : Processor, new()
    {
        if (processorMap.ContainsKey(typeof(T)))
            throw new InvalidOperationException(@"The given processor type is already registered to this collection.");

        var processor = Activator.CreateInstance<T>();

        processors.Add(processor);
        processorMap.Add(typeof(T), processor);
    }

    internal void Attach(Scene scene)
    {
        if (this.scene != null)
            throw new InvalidOperationException();

        this.scene = scene;

        foreach (var processor in processors)
            processor.Attach(scene);
    }

    internal void Detach(Scene scene)
    {
        if (this.scene != scene)
            throw new InvalidOperationException();

        foreach (var processor in processors)
            processor.Detach(scene);

        this.scene = null;
    }

    internal void Attach(IProcessorAttachable attachable)
    {
        var attachableType = attachable.GetType();

        if (!attachableMap.TryGetValue(attachableType, out var processorTypes))
        {
            var attribs = attachableType.GetCustomAttributes<ProcessorAttribute>(true);
            processorTypes = attribs.SelectMany(attr => attr.GetType().GetGenericArguments()).ToArray();
            attachableMap.Add(attachableType, processorTypes);
        }

        foreach (var processorType in processorTypes)
        {
            if (processorMap.TryGetValue(processorType, out var processor))
                processor.Attach(attachable);
        }
    }

    internal void Detach(IProcessorAttachable attachable)
    {
        var attachableType = attachable.GetType();

        if (!attachableMap.TryGetValue(attachableType, out var processorTypes))
            return;

        foreach (var processorType in processorTypes)
        {
            if (processorMap.TryGetValue(processorType, out var processor))
                processor.Detach(attachable);
        }
    }

    public IEnumerator<Processor> GetEnumerator() => processors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
