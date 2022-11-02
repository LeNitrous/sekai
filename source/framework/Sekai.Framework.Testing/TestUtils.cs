// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Reflection;

namespace Sekai.Framework.Testing;

internal static class TestUtils
{
    public static bool IsNUnit => isNUnit.Value;
    private static readonly Lazy<bool> isNUnit = new(() => Assembly.GetEntryAssembly()?.Location.Contains("testhost") ?? false);
}
