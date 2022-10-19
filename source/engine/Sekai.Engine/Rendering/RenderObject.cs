// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Framework;

namespace Sekai.Engine.Rendering;

/// <summary>
/// A renderable object.
/// </summary>
public class RenderObject : FrameworkObject
{
    /// <summary>
    /// The world matrix for this render object.
    /// </summary>
    public Matrix4x4 WorldMatrix = Matrix4x4.Identity;
}
