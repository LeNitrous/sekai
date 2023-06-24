// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Headless;
using Sekai.Hosting;

[assembly: AudioProvider(typeof(DummyProvider))]
[assembly: PlatformProvider(typeof(DummyProvider))]
[assembly: GraphicsProvider(typeof(DummyProvider))]
