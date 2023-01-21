// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.ComponentModel;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// Global uniforms used by Sekai.
/// </summary>
public enum GlobalUniforms
{
    /// <summary>
    /// The projection matrix.
    /// </summary>
    [Description("g_internal_ProjMatrix")]
    Projection,

    /// <summary>
    /// The view matrix.
    /// </summary>
    [Description("g_internal_ViewMatrix")]
    View,

    /// <summary>
    /// The model matrix.
    /// </summary>
    [Description("g_internal_ModelMatrix")]
    Model,
}
