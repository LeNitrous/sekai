// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sekai.Scenes;

namespace Sekai.Processors;

internal class ProcessorManager : DisposableObject
{
    private readonly Dictionary<Type, Processor> processors = new();
    private readonly Dictionary<Type, List<Processor>> componentProcessorList = new();

    public void Attach(Component item)
    {
        var type = item.GetType();

        if (!componentProcessorList.TryGetValue(type, out var processorList))
        {
            processorList = new();

            foreach (var attrib in type.GetCustomAttributes<ProcessorAttribute>(true))
            {
                if (!processors.TryGetValue(attrib.Type, out var processor))
                {
                    processor = attrib.CreateInstance();
                    processors[attrib.Type] = processor;
                }

                processorList.Add(processor);
                processorList.Sort(processorComparer);
            }

            componentProcessorList[type] = processorList;
        }

        foreach (var processor in processorList)
            processor.OnComponentAttach(item);
    }

    public void Detach(Component item)
    {
        var type = item.GetType();

        if (!componentProcessorList.TryGetValue(type, out var processorList))
            throw new InvalidOperationException(@"Attempted to detach an unregistered component type.");

        foreach (var processor in processorList)
            processor.OnComponentDetach(item);
    }

    public void Update(Component item)
    {
        var type = item.GetType();

        if (!componentProcessorList.TryGetValue(type, out var processorList))
            throw new InvalidOperationException(@"Attempted to update an unregistered component type.");

        foreach (var processor in processorList)
            processor.Update(item);
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        componentProcessorList.Clear();

        foreach (var processor in processors.Values)
            processor.Dispose();

        processors.Clear();
    }

    private static readonly IComparer<Processor> processorComparer = Comparer<Processor>.Create((a, b) => a.Priority.CompareTo(b.Priority));
}
