// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ResolvedAttribute : Attribute
{
}
