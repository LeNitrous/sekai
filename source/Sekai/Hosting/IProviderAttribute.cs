// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Hosting;

internal interface IProviderAttribute
{
    Type Type { get; }
}
