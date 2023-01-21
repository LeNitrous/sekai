// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sekai.Scenes;

namespace Sekai.Processors;

public sealed class ProcessorManager : DependencyObject
{
    /// <summary>
    /// Gets all the processors currently loaded on this manager.
    /// </summary>
    public IEnumerable<Processor> Processors => processors;

    private readonly List<Processor> processors = new();
    private readonly Dictionary<Type, Processor> processorMapping = new();
    private readonly Dictionary<Type, ProcessorAttribute[]> processorsForComponent = new();

    /// <summary>
    /// Gets processors for a given component.
    /// </summary>
    public IEnumerable<Processor> GetComponentProcessors(Component component)
    {
        var type = component.GetType();

        if (!processorsForComponent.TryGetValue(type, out var attribs))
        {
            attribs = type.GetCustomAttributes<ProcessorAttribute>(true).ToArray();
            processorsForComponent.Add(type, attribs);
        }

        foreach (var attrib in attribs)
        {
            if (!processorMapping.TryGetValue(attrib.Type, out var processor))
            {
                processor = attrib.CreateInstance();
                processorMapping.Add(attrib.Type, processor);

                processors.Add(processor);
                processors.Sort(processorComparer);
            }

            yield return processor;
        }
    }

    private static readonly IComparer<Processor> processorComparer = Comparer<Processor>.Create((a, b) => a.Priority.CompareTo(b.Priority));
}
