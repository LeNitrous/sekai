// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Reflection;

namespace Sekai.Framework.Testing;

internal class TestUtils
{
    public static bool IsNUnit => isTesting.Value;
    private static readonly Lazy<bool> isTesting = new(() => Assembly.GetEntryAssembly()?.Location.Contains("testhost") ?? false);
}
