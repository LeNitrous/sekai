// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// The stage of shader code.
/// </summary>
[Flags]
public enum ShaderStage
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// This shader is a vertex shader.
    /// </summary>
    Vertex = 1 << 0,

    /// <summary>
    /// This shader is a geometry shader.
    /// </summary>
    Geometry = 1 << 1,

    /// <summary>
    /// This shader is a tesselation control shader.
    /// </summary>
    TesselationControl = 1 << 2,

    /// <summary>
    /// This shader is a tesselation evaluation shader.
    /// </summary>
    TesselationEvaluation = 1 << 3,

    /// <summary>
    /// This shader is a fragment shader.
    /// </summary>
    Fragment = 1 << 4,

    /// <summary>
    /// This shader is a compute shader.
    /// </summary>
    Compute = 1 << 5,
}
