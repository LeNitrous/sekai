// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sekai.Input;

/// <summary>
/// An input source capable of producing input events.
/// </summary>
public class InputSource : IInputSource
{
    private readonly ConcurrentQueue<EventData> events = new();

    /// <summary>
    /// Enqueues a mouse button event.
    /// </summary>
    protected void Enqueue(MouseButtonEvent e)
    {
        events.Enqueue(new EventData
        {
            Kind = EventKind.MouseButton,
            MouseButton = e,
        });
    }

    /// <summary>
    /// Enqueues a mouse motion event.
    /// </summary>
    protected void Enqueue(MouseMotionEvent e)
    {
        events.Enqueue(new EventData
        {
            Kind = EventKind.MouseMotion,
            MouseMotion = e,
        });
    }

    /// <summary>
    /// Enqueues a mouse scroll event.
    /// </summary>
    protected void Enqueue(MouseScrollEvent e)
    {
        events.Enqueue(new EventData
        {
            Kind = EventKind.MouseScroll,
            MouseScroll = e,
        });
    }

    /// <summary>
    /// Enqueues a keyboard event.
    /// </summary>
    protected void Enqueue(KeyboardEvent e)
    {
        events.Enqueue(new EventData
        {
            Kind = EventKind.Keyboard,
            Keyboard = e,
        });
    }

    IEnumerable<EventData> IInputSource.PumpEvents()
    {
        while (events.TryDequeue(out var e))
        {
            yield return e;
        }
    }
}

/// <summary>
/// An input source capable of pumping events to a consumer.
/// </summary>
internal interface IInputSource
{
    /// <summary>
    /// Pumps all events to an enumerable collection of event data.
    /// </summary>
    /// <returns>An enumerable collection of event data.</returns>
    IEnumerable<EventData> PumpEvents();
}
