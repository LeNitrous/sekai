// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Shaders;

/// <summary>
/// Result of transpiling shader code.
/// </summary>
public struct ShaderTranspileResult
{
    /// <summary>
    /// The transpiled vertex shader code.
    /// </summary>
    public string? Vertex { get; set; }

    /// <summary>
    /// The transpiled fragment shader code.
    /// </summary>
    public string? Fragment { get; set; }

    /// <summary>
    /// The transpiled compute shader code.
    /// </summary>
    public string? Compute { get; set; }
}
