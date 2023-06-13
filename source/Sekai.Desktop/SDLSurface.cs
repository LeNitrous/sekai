// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Platform;
using static SDL2.SDL;

namespace Sekai.Desktop;

internal readonly struct SDLSurface : ISurface
{
    public SurfaceFlags Flags { get; }
    public (nint Display, nuint Window)? X11 { get; }
    public nint? Cocoa { get; }
    public (nint Display, nint Surface)? Wayland { get; }
    public nint? WinRT { get; }
    public nint? Mir { get; }
    public (nint Window, nint Surface)? DirectFramebuffer { get; }
    public (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit { get; }
    public (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; }
    public (nint Display, nint Window)? Vivante { get; }
    public (nint Window, nint Surface)? Android { get; }
    public IGLContextSource? GL { get; }

    public SDLSurface(nint window)
    {
        SDL_SysWMinfo info = default;
        SDL_GetWindowWMInfo(window, ref info);

        Flags |= SurfaceFlags.OpenGL;
        GL = new GLContextSource(window);

        switch (info.subsystem)
        {
            case SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS:
                Flags |= SurfaceFlags.Win32;
                Win32 = (info.info.win.window, info.info.win.hdc, info.info.win.hinstance);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_X11:
                Flags |= SurfaceFlags.X11;
                X11 = (info.info.x11.display, (nuint)info.info.x11.window);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_DIRECTFB:
                Flags |= SurfaceFlags.DirectFramebuffer;
                DirectFramebuffer = (info.info.dfb.window, info.info.dfb.surface);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_COCOA:
                Flags |= SurfaceFlags.Cocoa;
                Cocoa = info.info.cocoa.window;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_UIKIT:
                Flags |= SurfaceFlags.UIKit;
                UIKit = (info.info.uikit.window, info.info.uikit.framebuffer, info.info.uikit.colorbuffer, info.info.uikit.resolveFramebuffer);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_WAYLAND:
                Flags |= SurfaceFlags.Wayland;
                Wayland = (info.info.wl.display, info.info.wl.surface);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_MIR:
                Flags |= SurfaceFlags.Mir;
                Mir = info.info.mir.surface;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_WINRT:
                Flags |= SurfaceFlags.WinRT;
                WinRT = info.info.winrt.window;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_ANDROID:
                Flags |= SurfaceFlags.Android;
                Android = (info.info.android.window, info.info.android.surface);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_VIVANTE:
                Flags |= SurfaceFlags.Vivante;
                Vivante = (info.info.vivante.display, info.info.vivante.window);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(window), window, null);
        }
    }
}
