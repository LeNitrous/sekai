// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// Exception thrown when <see cref="IServiceLocator"/> fails to locate a required service of a given type.
/// </summary>
public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException()
    {
    }

    public ServiceNotFoundException(Type type)
        : base($"Failed to locate service of type {type}.")
    {
    }

    public ServiceNotFoundException(string? message)
        : base(message)
    {
    }

    public ServiceNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
