// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public class ShaderCompilationOptions
{
    public string Filename { get; set; }
    public ShaderConstant[] Constants { get; set; }
    public Dictionary<string, string> Macros { get; set; }

    public ShaderCompilationOptions(string filename = "file", ShaderConstant[]? constants = null, Dictionary<string, string>? macros = null)
    {
        Macros = macros ?? new Dictionary<string, string>();
        Filename = filename;
        Constants = constants ?? Array.Empty<ShaderConstant>();
    }
}
