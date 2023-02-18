// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Processors;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public abstract class ProcessorAttribute : Attribute
{
    public readonly Type Type;

    private readonly Func<Processor> creator;

    protected ProcessorAttribute(Type type, Func<Processor> creator)
    {
        Type = type;
        this.creator = creator;
    }

    internal Processor CreateInstance() => creator();
}


public sealed class ProcessorAttribute<T> : ProcessorAttribute
    where T : Processor, new()
{
    public ProcessorAttribute()
        : base(typeof(T), static () => new T())
    {
    }
}
