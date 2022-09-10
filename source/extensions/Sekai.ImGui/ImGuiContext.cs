// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using ImGuiNET;
using Sekai.Framework;
using Gui = ImGuiNET.ImGui;

namespace Sekai.ImGui;

internal class ImGuiContext : FrameworkObject
{
    public nint Handle { get; private set; }

    public ImGuiContext()
        : this(IntPtr.Zero)
    {
    }

    public ImGuiContext(ImFontAtlasPtr sharedData)
    {
        var previousContext = Gui.GetCurrentContext();

        Gui.SetCurrentContext(IntPtr.Zero);
        Handle = Gui.CreateContext(sharedData);

        Gui.SetCurrentContext(previousContext);
    }

    public void MakeCurrent()
    {
        if (Gui.GetCurrentContext() != Handle)
            Gui.SetCurrentContext(Handle);
    }

    protected override void Destroy()
    {
        Gui.SetCurrentContext(IntPtr.Zero);
        Gui.DestroyContext(Handle);
    }
}
