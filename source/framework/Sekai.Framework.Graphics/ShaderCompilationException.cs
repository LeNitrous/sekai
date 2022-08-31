using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Represents errors that occur during shader compilation.
/// </summary>
public class ShaderCompilationException : Exception
{
    public ShaderCompilationException(string? message)
        : base(message)
    {
    }
}
