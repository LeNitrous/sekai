// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

[Flags]
public enum LayoutElementFlags
{
    None,
    DynamicBinding = 1 << 0,
}
