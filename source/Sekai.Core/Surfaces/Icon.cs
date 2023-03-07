// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Surfaces;

public readonly record struct Icon(Size Size, ReadOnlyMemory<byte> Data);
