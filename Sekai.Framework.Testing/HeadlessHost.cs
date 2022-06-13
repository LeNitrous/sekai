// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework.Internal;
using Sekai.Framework.Platform;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Testing;

internal class HeadlessHost : Host
{
    protected override GameThread CreateMainThread()
    {
        return new GameThread("Main");
    }

    protected sealed override GameThreadManager CreateThreadManager()
    {
        return new HeadlessThreadManager();
    }

    private class HeadlessThreadManager : GameThreadManager
    {
        public override void Add(GameThread thread)
        {
            var context = TestExecutionContext.CurrentContext;

            thread.Dispatch(() =>
            {
                TestExecutionContext.CurrentContext.CurrentResult = context.CurrentResult;
                TestExecutionContext.CurrentContext.CurrentTest = context.CurrentTest;
                TestExecutionContext.CurrentContext.CurrentCulture = context.CurrentCulture;
                TestExecutionContext.CurrentContext.CurrentPrincipal = context.CurrentPrincipal;
                TestExecutionContext.CurrentContext.CurrentRepeatCount = context.CurrentRepeatCount;
                TestExecutionContext.CurrentContext.CurrentUICulture = context.CurrentUICulture;
            });

            base.Add(thread);
        }
    }
}
