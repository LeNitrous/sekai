// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sekai.Allocation;
using Sekai.Processors;

namespace Sekai.Scenes;

public sealed class ComponentManager : DependencyObject
{
    [Resolved]
    private ProcessorManager processorManager { get; set; } = null!;

    private readonly Queue<ComponentEvent> events = new();
    private readonly Dictionary<Type, List<Component>> componentMapping = new();

    public void Update()
    {
        while (events.TryDequeue(out var e))
        {
            switch (e.Type)
            {
                case ComponentEventType.Attach:
                    attachComponent(e.Component);
                    break;

                case ComponentEventType.Detach:
                    detachComponent(e.Component);
                    break;
            }
        }

        foreach (var processor in processorManager.Processors)
        {
            var components = componentMapping[processor.GetType()];

            if (components.Count < 0)
                continue;

            foreach (var component in CollectionsMarshal.AsSpan(components))
                processor.Update(component);
        }

        void attachComponent(Component item)
        {
            foreach (var processor in processorManager.GetComponentProcessors(item))
            {
                var type = processor.GetType();

                if (!componentMapping.TryGetValue(type, out var components))
                {
                    components = new();
                    componentMapping.Add(type, components);
                }

                components.Add(item);
                processor.OnComponentAttach(item);
            }
        }

        void detachComponent(Component item)
        {
            foreach (var processor in processorManager.GetComponentProcessors(item))
            {
                if (!componentMapping.TryGetValue(processor.GetType(), out var components))
                    continue;

                if (!components.Remove(item))
                    continue;

                processor.OnComponentDetach(item);
            }
        }
    }

    public void Add(Node node)
    {
        foreach (var component in (IEnumerable<Component>)node)
            add(component, component.IsReady);
    }

    public void Remove(Node node)
    {
        foreach (var component in (IEnumerable<Component>)node)
            remove(component, true);
    }

    public void Add(Component component)
    {
        add(component, false);
    }

    public void Remove(Component component)
    {
        remove(component, false);
    }

    private void add(Component component, bool immediately)
    {
        component.OnStateChanged += onComponentStateChange;

        if (immediately)
            events.Enqueue(new(ComponentEventType.Attach, component));
    }

    private void remove(Component component, bool immediately)
    {
        component.OnStateChanged -= onComponentStateChange;

        if (immediately)
            events.Enqueue(new(ComponentEventType.Detach, component));
    }

    private void onComponentStateChange(Component sender, bool state)
    {
        if (state)
        {
            events.Enqueue(new(ComponentEventType.Attach, sender));
        }
        else
        {
            events.Enqueue(new(ComponentEventType.Detach, sender));
        }
    }

    private readonly record struct ComponentEvent(ComponentEventType Type, Component Component);

    private enum ComponentEventType
    {
        Attach,
        Detach,
    }
}
