// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Platform;
using static SDL2.SDL;

namespace Sekai.Desktop;

internal readonly struct GLContextSource : IGLContextSource
{
    private readonly nint window;

    public GLContextSource(nint window)
    {
        this.window = window;
    }

    public void ClearCurrentContext()
    {
        if (SDL_GL_MakeCurrent(window, IntPtr.Zero) > 0)
            throw new Exception(SDL_GetError());
    }

    public nint CreateContext()
    {
        return SDL_GL_CreateContext(window);
    }

    public void DeleteContext(nint context)
    {
        SDL_GL_DeleteContext(context);
    }

    public nint GetCurrentContext()
    {
        return SDL_GL_GetCurrentContext();
    }

    public nint GetProcAddress(string proc)
    {
        return SDL_GL_GetProcAddress(proc);
    }

    public void MakeCurrent(nint context)
    {
        if (SDL_GL_MakeCurrent(window, context) > 0)
            throw new Exception(SDL_GetError());
    }

    public void SetSyncToVerticalBlank(SyncMode sync)
    {
        int value = sync switch
        {
            SyncMode.None => 0,
            SyncMode.Traditional => 1,
            SyncMode.Adaptive => -1,
            _ => throw new ArgumentOutOfRangeException(nameof(sync), sync, null)
        };

        if (SDL_GL_SetSwapInterval(value) < 0)
            throw new Exception(SDL_GetError());
    }

    public void SwapBuffers()
    {
        SDL_GL_SwapWindow(window);
    }
}
