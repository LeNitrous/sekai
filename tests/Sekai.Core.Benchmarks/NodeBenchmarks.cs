// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using BenchmarkDotNet.Attributes;

namespace Sekai.Core.Benchmarks;

[MemoryDiagnoser]
public class NodeBenchmarks
{
    [Params(10, 100, 1000)]
    public int Count { get; set; }

    [Benchmark]
    public Node Node()
    {
        var node = new Node();

        for (int i = 0; i < Count; i++)
        {
            node.Add(new Node());
        }

        return node;
    }
}
