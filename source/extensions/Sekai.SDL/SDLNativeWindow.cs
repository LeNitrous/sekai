// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Windowing;
using static SDL2.SDL;

namespace Sekai.SDL;

internal readonly struct SDLNativeWindow : INativeWindow
{
    public NativeWindowKind Kind { get; }
    public (nint Display, nint Window)? X11 { get; } = null!;
    public nint? Cocoa { get; } = null!;
    public (nint Display, nint Surface)? Wayland { get; } = null!;
    public nint? WinRT { get; } = null!;
    public (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit { get; } = null!;
    public (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; } = null!;
    public (nint Display, nint Window)? Vivante { get; } = null!;
    public (nint Window, nint Surface)? Android { get; } = null!;

    public SDLNativeWindow(SDLView view)
    {
        var info = new SDL_SysWMinfo();
        SDL_VERSION(out info.version);
        SDL_GetWindowWMInfo(view.Window, ref info);

        switch (info.subsystem)
        {
            default:
                {
                    Kind = NativeWindowKind.Unknown;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_OS2:
                {
                    Kind = NativeWindowKind.OS2;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_HAIKU:
                {
                    Kind = NativeWindowKind.Haiku;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_DIRECTFB:
                {
                    Kind = NativeWindowKind.DirectFramebuffer;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS:
                {
                    Kind = NativeWindowKind.Win32;
                    Win32 = (info.info.win.window, info.info.win.hdc, info.info.win.hinstance);
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_X11:
                {
                    Kind = NativeWindowKind.X11;
                    X11 = (info.info.x11.display, info.info.x11.window);
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_COCOA:
                {
                    Kind = NativeWindowKind.Cocoa;
                    Cocoa = info.info.cocoa.window;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_UIKIT:
                {
                    Kind = NativeWindowKind.UIKit;
                    UIKit = (info.info.uikit.window, info.info.uikit.framebuffer, info.info.uikit.colorbuffer, info.info.uikit.resolveFramebuffer);
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_WAYLAND:
                {
                    Kind = NativeWindowKind.Wayland;
                    Wayland = (info.info.wl.display, info.info.wl.surface);
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_WINRT:
                {
                    Kind = NativeWindowKind.WinRT;
                    WinRT = info.info.winrt.window;
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_ANDROID:
                {
                    Kind = NativeWindowKind.Android;
                    Android = (info.info.android.window, info.info.android.surface);
                    break;
                }

            case SDL_SYSWM_TYPE.SDL_SYSWM_VIVANTE:
                {
                    Kind = NativeWindowKind.Vivante;
                    Vivante = (info.info.vivante.display, info.info.vivante.window);
                    break;
                }
        }
    }
}
