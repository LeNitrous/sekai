﻿// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Sekai.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
public class BenchmarkComponentRetrieval
{
    private readonly Entity entity = new();
    private readonly Type type = typeof(TestComponent);

    [GlobalSetup]
    public void Setup()
    {
        entity.AddComponent<TestComponent>();
    }

    [Benchmark]
    public void GetComponent() => entity.GetComponent(type);

    [Benchmark]
    public void GetComponentGeneric() => entity.GetComponent<TestComponent>();

    private class TestComponent : Component
    {
    }
}
