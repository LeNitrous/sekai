// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NetEscapades.EnumGenerators;

namespace Sekai.Graphics;

/// <summary>
/// The stage of shader code.
/// </summary>
[Flags, EnumExtensions]
public enum ShaderStage
{
    /// <summary>
    /// This shader is a vertex shader.
    /// </summary>
    Vertex,

    /// <summary>
    /// This shader is a geometry shader.
    /// </summary>
    Geometry,

    /// <summary>
    /// This shader is a tesselation control shader.
    /// </summary>
    TesselationControl,

    /// <summary>
    /// This shader is a tesselation evaluation shader.
    /// </summary>
    TesselationEvaluation,

    /// <summary>
    /// This shader is a fragment shader.
    /// </summary>
    Fragment,

    /// <summary>
    /// This shader is a compute shader.
    /// </summary>
    Compute,
}
