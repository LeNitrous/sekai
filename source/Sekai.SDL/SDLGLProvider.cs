// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Windowing.OpenGL;

namespace Sekai.SDL;

internal unsafe class SDLGLProvider : FrameworkObject, IOpenGLProvider
{
    public nint Handle { get; }

    private readonly SDLView view;

    public SDLGLProvider(SDLView view)
    {
        this.view = view;
        Handle = CreateContext();
    }

    public void ClearCurrentContext()
    {
        if (view.Sdl.GLMakeCurrent(null, null) != 0)
            view.Sdl.ThrowError();
    }

    public nint CreateContext()
    {
        void* context = view.Sdl.GLCreateContext(view.Window);

        if (context == null)
            view.Sdl.ThrowError();

        return (nint)context;
    }

    public void DeleteContext(nint context)
    {
        view.Sdl.GLDeleteContext((void*)context);
    }

    public nint GetCurrentContext()
    {
        return (nint)view.Sdl.GLGetCurrentContext();
    }

    public nint GetProcAddress(string name)
    {
        return (nint)view.Sdl.GLGetProcAddress(name);
    }

    public void MakeCurrent(nint context)
    {
        if (view.Sdl.GLMakeCurrent(view.Window, (void*)context) != 0)
            view.Sdl.ThrowError();
    }

    public void MakeCurrent()
    {
        MakeCurrent(Handle);
    }

    public void SetSyncToVerticalBlank(bool sync)
    {
        if (view.Sdl.GLSetSwapInterval(sync ? 1 : 0) != 0)
            view.Sdl.ThrowError();
    }

    public void SwapBuffers()
    {
        view.Sdl.GLSwapWindow(view.Window);
    }

    protected override void Destroy()
    {
        DeleteContext(Handle);
    }
}
