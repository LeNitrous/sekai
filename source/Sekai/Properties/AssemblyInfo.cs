// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Sekai;

[assembly: InternalsVisibleTo("Sekai.Tests")]
[assembly: MetadataUpdateHandler(typeof(HotReload))]
