// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Sekai.Xunit;

public class GameTestFramework : XunitTestFramework
{
    public GameTestFramework(IMessageSink messageSink)
        : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new GameTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
}
