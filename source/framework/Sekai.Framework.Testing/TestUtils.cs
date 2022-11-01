using System.Reflection;

namespace Sekai.Framework.Testing;

internal static class TestUtils
{
    public static bool IsNUnit => isNUnit.Value;
    private static readonly Lazy<bool> isNUnit = new(() => Assembly.GetEntryAssembly()?.Location.Contains("testhost") ?? false);
}
