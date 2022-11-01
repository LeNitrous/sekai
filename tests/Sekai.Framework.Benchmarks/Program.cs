// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using BenchmarkDotNet.Running;

namespace Sekai.Framework.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
