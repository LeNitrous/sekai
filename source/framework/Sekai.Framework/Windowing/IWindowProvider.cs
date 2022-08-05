// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.Framework.Windowing;

public interface IWindowProvider
{
    IReadOnlyList<IMonitor> Monitors { get; }
    IWindow CreateWindow();
    IInputContext CreateInput();
}
