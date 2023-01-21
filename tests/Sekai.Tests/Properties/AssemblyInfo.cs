// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Tests;
using Sekai.Xunit;
using Xunit;

[assembly: TestFramework("Sekai.Xunit.GameTestFramework", "Sekai.Xunit")]
[assembly: TestGameBuilder<TestGameBuilder>]
