// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Sekai.Windowing.OpenGL;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Graphics.OpenGL;
using static Windows.Win32.PInvoke;

namespace Sekai.Forms;

internal unsafe struct FormsGLContext : IOpenGLContext
{
    public nint Handle { get; }

    private nint module;
    private readonly HDC hdc;

    public FormsGLContext(HDC hdc)
    {
        this.hdc = hdc;

        PIXELFORMATDESCRIPTOR pfd;
        pfd.nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR);
        pfd.nVersion = 1;
        pfd.dwFlags = PFD_FLAGS.PFD_DRAW_TO_WINDOW | PFD_FLAGS.PFD_SUPPORT_OPENGL | PFD_FLAGS.PFD_DOUBLEBUFFER;
        pfd.iPixelType = PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
        pfd.cColorBits = 32;
        pfd.cDepthBits = 24;

        int format = ChoosePixelFormat(hdc, &pfd);
        SetPixelFormat(hdc, format, &pfd);

        Handle = CreateContext();

        MakeCurrent();
    }

    public void ClearCurrentContext()
    {
        wglMakeCurrent(HDC.Null, HGLRC.Null);
    }

    public nint CreateContext()
    {
        return wglCreateContext(hdc);
    }

    public void DeleteContext(nint context)
    {
        if (!wglDeleteContext((HGLRC)context))
            throw new InvalidOperationException($"Failed to delete context: {Marshal.GetLastWin32Error()}");
    }

    public nint GetCurrentContext()
    {
        return wglGetCurrentContext();
    }

    public nint GetProcAddress(string name)
    {
        nint addr = IntPtr.Zero;

        if ((addr = wglGetProcAddress(name)) != IntPtr.Zero)
            return addr;

        if (module == IntPtr.Zero)
            module = NativeLibrary.Load("opengl32.dll");

        return NativeLibrary.GetExport(module, name);
    }

    public void MakeCurrent(nint context)
    {
        wglMakeCurrent((HDC)hdc, (HGLRC)context);
    }

    public void MakeCurrent()
    {
        MakeCurrent(Handle);
    }

    public void SetSyncToVerticalBlank(bool sync)
    {
    }

    public void SwapBuffers()
    {
        PInvoke.SwapBuffers((HDC)hdc);
    }

    public void Dispose()
    {
        if (module != IntPtr.Zero)
            NativeLibrary.Free(module);
    }
}
