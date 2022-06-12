// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Logging;
using SharpDX;
using SharpDX.DXGI;
using Veldrid;

namespace Sekai.Framework.Graphics;

internal sealed class Direct3D11GraphicsContext : GraphicsContext
{
    protected override GraphicsBackend Backend => GraphicsBackend.Direct3D11;

    protected override unsafe void Initialize()
    {
        var info = Device.GetD3D11Info();
        var adapter = CppObject.FromPointer<Adapter>(info.Adapter);

        Logger.Log($@"Direct3D 11 Inititalized");
        Logger.Log($@"Direct3D 11 Adapter:                 {adapter.Description.Description}");
        Logger.Log($@"Direct3D 11 Dedicated Video Memory:  {adapter.Description.DedicatedVideoMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Dedicated System Memory: {adapter.Description.DedicatedSystemMemory / 1024 / 1024} MB");
        Logger.Log($@"Direct3D 11 Shared System Memory:    {adapter.Description.SharedSystemMemory / 1024 / 1024} MB");
    }
}
