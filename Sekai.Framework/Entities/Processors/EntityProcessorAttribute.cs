// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Entities.Processors;

[AttributeUsage(AttributeTargets.Class)]
public class EntityProcessorAttribute : Attribute
{
    public readonly Type ProcessorType;

    public EntityProcessorAttribute(Type processorType)
    {
        if (!processorType.IsAssignableTo(typeof(EntityProcessor)))
            throw new InvalidCastException($@"{processorType} is not a valid processor type.");

        if (processorType.GetConstructor(Type.EmptyTypes) == null)
            throw new TypeLoadException(@$"{processorType} must have a parameterless constructor.");

        ProcessorType = processorType;
    }
}
