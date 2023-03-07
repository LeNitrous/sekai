// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Surfaces;

public readonly record struct VideoMode(Size Resolution, int RefreshRate, int BitsPerPixel);
