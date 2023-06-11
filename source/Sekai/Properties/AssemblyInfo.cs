// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Sekai.Platform;

[assembly: InternalsVisibleTo("Sekai.Tests")]
[assembly: InternalsVisibleTo("Sekai.Benchmarks")]
[assembly: MetadataUpdateHandler(typeof(HotReloadCallbackReceiver))]
