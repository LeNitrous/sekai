// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics.Textures;

public enum SamplerFilter
{
    MinPoint_MagPoint_MipPoint,
    MinPoint_MagPoint_MipLinear,
    MinPoint_MagLinear_MipPoint,
    MinPoint_MagLinear_MipLinear,
    MinLinear_MagPoint_MipPoint,
    MinLinear_MagPoint_MipLinear,
    MinLinear_MagLinear_MipPoint,
    MinLinear_MagLinear_MipLinear,
    Anisotropic,
}
