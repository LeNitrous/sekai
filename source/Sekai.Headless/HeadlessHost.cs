// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Storages;
using Sekai.Framework.Windowing;
using Sekai.Headless.Audio;
using Sekai.Headless.Graphics;
using Sekai.Headless.Input;
using Sekai.Headless.Windowing;

namespace Sekai.Headless;

public class HeadlessHost : Host
{
    public override IEnumerable<IMonitor> Monitors => Enumerable.Empty<IMonitor>();
    protected override IInputContext CreateInput(IWindow window) => new DummyInputContext();
    protected override Storage CreateStorage(MountTarget target) => new MemoryStorage();
    protected override IWindow CreateWindow() => new DummyWindow();
    protected override AudioDevice CreateAudio() => new DummyAudioDevice();
    protected override GraphicsDevice CreateGraphics(IWindow window) => new DummyGraphicsDevice();
}