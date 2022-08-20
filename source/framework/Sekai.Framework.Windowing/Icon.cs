// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Windowing;

public readonly record struct Icon(int Width, int Height, Memory<byte> Data);
