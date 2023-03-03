// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// Represents an error when a <see cref="Component"/> is not found on a <see cref="Node"/>.
/// </summary>
public class ComponentNotFoundException : Exception
{
    public ComponentNotFoundException()
    {
    }

    public ComponentNotFoundException(string? message)
        : base(message)
    {
    }

    public ComponentNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
