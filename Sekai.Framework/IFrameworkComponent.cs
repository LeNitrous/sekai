// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework;
public interface IFrameworkComponent : IDisposable
{
    protected virtual void Destroy() { }
}
