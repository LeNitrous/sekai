// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Platform.Input;

/// <summary>
/// An interface for objects that are capable of producing input <see cref="EventData"/>.
/// </summary>
public interface IInputSource
{
    /// <summary>
    /// Returns a single <see cref="EventData"/> from a queue.
    /// </summary>
    /// <param name="data">The pumped event.</param>
    /// <returns><see langword="true"/> if an event has been pumped. Otherwise, returns <see langword="false"/>.</returns>
    bool PumpEvent(out EventData data);
}
