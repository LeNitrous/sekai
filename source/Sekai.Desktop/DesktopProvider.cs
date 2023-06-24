// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Hosting;

namespace Sekai.Desktop;

internal sealed class DesktopProvider : IPlatformProvider
{
    public Platform CreatePlatform()
    {
        if (RuntimeInfo.IsDesktop)
        {
            return new DesktopPlatform();
        }

        throw new PlatformNotSupportedException();
    }
}
