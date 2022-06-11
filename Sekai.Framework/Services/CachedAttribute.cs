// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Services;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CachedAttribute : Attribute
{
}
