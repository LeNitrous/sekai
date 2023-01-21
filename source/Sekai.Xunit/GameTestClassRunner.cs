// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Sekai.Xunit;

internal class GameTestClassRunner : XunitTestClassRunner
{
    private readonly ITestGameBuilder builder;

    public GameTestClassRunner(ITestGameBuilder builder, ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings)
        : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
    {
        this.builder = builder;
    }

    protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
        => new GameTestMethodRunner(builder, testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, Aggregator, CancellationTokenSource, constructorArguments).RunAsync();
}
