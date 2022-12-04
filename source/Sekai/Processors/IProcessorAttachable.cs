// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Scenes;

namespace Sekai.Processors;

/// <summary>
/// Denotes that an object is attachable to a <see cref="Processor"/>.
/// </summary>
public interface IProcessorAttachable
{
    /// <summary>
    /// The scene where this object is attached to.
    /// </summary>
    Scene? Scene { get; }
}
