// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Processors;

public abstract class ProcessorAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ProcessorAttribute<T> : ProcessorAttribute
    where T : Processor
{
}
