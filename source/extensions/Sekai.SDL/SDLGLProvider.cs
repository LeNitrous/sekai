// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Windowing.OpenGL;
using static SDL2.SDL;

namespace Sekai.SDL;

internal class SDLGLProvider : FrameworkObject, IOpenGLProvider
{
    public nint Handle { get; }

    private readonly SDLView view;

    public SDLGLProvider(SDLView view)
    {
        this.view = view;

        if (SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_COMPATIBILITY) > 0)
            throw new InvalidOperationException(SDL_GetError());

        Handle = CreateContext();
    }

    public void ClearCurrentContext()
    {
        if (SDL_GL_MakeCurrent(IntPtr.Zero, IntPtr.Zero) != 0)
            throw new InvalidOperationException(SDL_GetError());
    }

    public nint CreateContext()
    {
        nint context = SDL_GL_CreateContext(view.Window);

        if (context == IntPtr.Zero)
            throw new InvalidOperationException(SDL_GetError());

        return context;
    }

    public void DeleteContext(nint context)
    {
        SDL_GL_DeleteContext(context);
    }

    public nint GetCurrentContext()
    {
        return SDL_GL_GetCurrentContext();
    }

    public nint GetProcAddress(string name)
    {
        return SDL_GL_GetProcAddress(name);
    }

    public void MakeCurrent(nint context)
    {
        if (SDL_GL_MakeCurrent(view.Window, context) != 0)
            throw new InvalidOperationException(SDL_GetError());
    }

    public void MakeCurrent()
    {
        MakeCurrent(Handle);
    }

    public void SetSyncToVerticalBlank(bool sync)
    {
        if (SDL_GL_SetSwapInterval(sync ? 1 : 0) != 0)
            throw new InvalidOperationException(SDL_GetError());
    }

    public void SwapBuffers()
    {
        SDL_GL_SwapWindow(view.Window);
    }

    protected override void Destroy()
    {
        DeleteContext(Handle);
    }
}
