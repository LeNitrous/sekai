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
    [Description("g_ProjMatrix")]
    Projection,

    /// <summary>
    /// The view matrix.
    /// </summary>
    [Description("g_ViewMatrix")]
    View,
}
