// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Windowing;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal readonly unsafe struct SDLNativeWindow : INativeWindow
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

    public SDLNativeWindow(SDLSurface view)
    {
        var info = new SysWMInfo();
        var vers = new Version();
        view.Sdl.GetVersion(ref vers);
        view.Sdl.GetWindowWMInfo(view.Window, &info);

        switch (info.Subsystem)
        {
            default:
                {
                    Kind = NativeWindowKind.Unknown;
                    break;
                }

            case SysWMType.OS2:
                {
                    Kind = NativeWindowKind.OS2;
                    break;
                }

            case SysWMType.Haiku:
                {
                    Kind = NativeWindowKind.Haiku;
                    break;
                }

            case SysWMType.DirectFB:
                {
                    Kind = NativeWindowKind.DirectFramebuffer;
                    break;
                }

            case SysWMType.Windows:
                {
                    Kind = NativeWindowKind.Win32;
                    Win32 = (info.Info.Win.Hwnd, info.Info.Win.HDC, info.Info.Win.HInstance);
                    break;
                }

            case SysWMType.X11:
                {
                    Kind = NativeWindowKind.X11;
                    X11 = ((nint)info.Info.X11.Display, (nint)info.Info.X11.Window);
                    break;
                }

            case SysWMType.Cocoa:
                {
                    Kind = NativeWindowKind.Cocoa;
                    Cocoa = (nint)info.Info.Cocoa.Window;
                    break;
                }

            case SysWMType.UIKit:
                {
                    Kind = NativeWindowKind.UIKit;
                    UIKit = ((nint)info.Info.UIKit.Window, info.Info.UIKit.Framebuffer, info.Info.UIKit.Colorbuffer, info.Info.UIKit.ResolveFramebuffer);
                    break;
                }

            case SysWMType.Wayland:
                {
                    Kind = NativeWindowKind.Wayland;
                    Wayland = ((nint)info.Info.Wayland.Display, (nint)info.Info.Wayland.Surface);
                    break;
                }

            case SysWMType.WinRT:
                {
                    Kind = NativeWindowKind.WinRT;
                    WinRT = (nint)info.Info.WinRT.Window;
                    break;
                }

            case SysWMType.Android:
                {
                    Kind = NativeWindowKind.Android;
                    Android = ((nint)info.Info.Android.Window, (nint)info.Info.Android.Surface);
                    break;
                }

            case SysWMType.Vivante:
                {
                    Kind = NativeWindowKind.Vivante;
                    Vivante = ((nint)info.Info.Vivante.Display, (nint)info.Info.Vivante.Window);
                    break;
                }
        }
    }
}
