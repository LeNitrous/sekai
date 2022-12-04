// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Rendering;

/// <summary>
/// An interface providing access to common transform properties.
/// </summary>
public interface ITransform
{
    /// <summary>
    /// The transform's local matrix.
    /// </summary>
    Matrix4x4 LocalMatrix { get; }

    /// <summary>
    /// The transform's world matrix.
    /// </summary>
    Matrix4x4 WorldMatrix { get; }

    /// <summary>
    /// The transform's world matrix inversed.
    /// </summary>
    Matrix4x4 WorldMatrixInverse { get; }
}
