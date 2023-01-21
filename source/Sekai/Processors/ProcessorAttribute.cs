// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Scenes;

namespace Sekai.Processors;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ProcessorAttribute : Attribute
{
    public readonly Type Type;

    public ProcessorAttribute(Type type)
    {
        Type = type;
    }

    internal Processor CreateInstance()
    {
        if (creatorMap.TryGetValue(Type, out var creatorFunc))
            return creatorFunc();

        var ctor = Type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);

        if (ctor is null)
            throw new InvalidOperationException($@"Processor type ""{Type}"" must have a constructor with exactly one argument of {nameof(Scene)}.");

        var expr = Expression.New(ctor);
        var lmbd = Expression.Lambda<Func<Processor>>(expr, false).Compile();

        creatorMap.Add(Type, lmbd);
        return lmbd();
    }

    private static readonly Dictionary<Type, Func<Processor>> creatorMap = new();
}


public sealed class ProcessorAttribute<T> : ProcessorAttribute
    where T : Processor
{
    public ProcessorAttribute()
        : base(typeof(T))
    {
    }
}
