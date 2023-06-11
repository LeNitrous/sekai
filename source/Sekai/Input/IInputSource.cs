// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Input;

public interface IInputSource
{
    /// <summary>
    /// Pumps all events to an enumerable collection of event data.
    /// </summary>
    /// <returns>An enumerable collection of event data.</returns>
    IEnumerable<EventData> PumpEvents();
}
