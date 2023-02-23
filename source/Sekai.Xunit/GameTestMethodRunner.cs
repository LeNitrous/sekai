// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Sekai.Xunit;

internal class GameTestMethodRunner : XunitTestMethodRunner
{
    private readonly ITestGameBuilder builder;

    public GameTestMethodRunner(ITestGameBuilder builder, ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments)
        : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
    {
        this.builder = builder;
    }

    protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
    {
        // FIXME: Run game isolated for tests.
        //var source = new TaskCompletionSource<RunSummary>();
        //var game = builder.Build();

        //game.OnLoad += () =>
        //{
        //    try
        //    {
        //        var task = base.RunTestCaseAsync(testCase);

        //        Task.Run(async () =>
        //        {
        //            try
        //            {
        //                await task;
        //            }
        //            catch (Exception e)
        //            {
        //                source.SetException(e);
        //            }
        //        });

        //        if (task.IsFaulted)
        //        {
        //            source.SetException(task.Exception!);
        //        }
        //        else if (task.IsCanceled)
        //        {
        //            source.SetCanceled();
        //        }
        //        else
        //        {
        //            source.SetResult(task.Result);
        //        }

        //        game.Exit();
        //    }
        //    catch (Exception e)
        //    {
        //        source.SetException(e);
        //    }
        //};

        //Task.Run(game.Run);

        //return source.Task;

        return base.RunTestCaseAsync(testCase);
    }
}
