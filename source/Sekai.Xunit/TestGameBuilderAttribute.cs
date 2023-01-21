// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Xunit;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class TestGameBuilderAttribute : Attribute
{
    public readonly ITestGameBuilder Builder;

    public TestGameBuilderAttribute(Type type)
    {
        if (!type.IsAssignableTo(typeof(ITestGameBuilder)))
            throw new ArgumentException($"{type} must implement {nameof(ITestGameBuilder)}.", nameof(type));

        Builder = (ITestGameBuilder)Activator.CreateInstance(type)!;
    }
}

public class TestGameBuilderAttribute<T> : TestGameBuilderAttribute
    where T : ITestGameBuilder
{
    public TestGameBuilderAttribute()
        : base(typeof(T))
    {
    }
}
