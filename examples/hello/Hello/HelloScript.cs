// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;
using Sekai.Logging;

namespace Hello;

public class HelloScript : Script
{
    public override void Start()
    {
        // See the output in console!
        Logger.Log("Hello World!");
    }
}
