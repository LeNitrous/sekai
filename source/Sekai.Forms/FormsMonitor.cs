// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Windows.Forms;
using Sekai.Mathematics;
using Sekai.Windowing;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace Sekai.Forms;

internal struct FormsMonitor : IMonitor
{
    public int Index { get; }
    public string Name => screen.DeviceName;
    public Rectangle Bounds => new(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
    public IReadOnlyList<VideoMode> Modes => modes;
    private readonly Screen screen;
    private readonly List<VideoMode> modes = new();

    public FormsMonitor(int index, Screen screen)
    {
        Index = index;
        this.screen = screen;

        int i = 0;
        DEVMODEW dev = default;

        while (PInvoke.EnumDisplaySettings(screen.DeviceName, (ENUM_DISPLAY_SETTINGS_MODE)i, ref dev))
        {
            modes.Add(new VideoMode(new Size2((int)dev.dmPelsWidth, (int)dev.dmPelsHeight), (int)dev.dmDisplayFrequency, (int)dev.dmBitsPerPel));
            i++;
        }
    }
}
