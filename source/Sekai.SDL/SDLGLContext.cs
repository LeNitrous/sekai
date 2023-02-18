// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Windowing.OpenGL;

namespace Sekai.SDL;

internal class SDLGLContext : DisposableObject, IOpenGLContext
{
    private readonly nint handle;
    private readonly SDLSurface view;

    public SDLGLContext(SDLSurface view, nint handle)
    {
        this.view = view;
        this.handle = handle;
    }

    public void MakeCurrent() => view.SetCurrentContext(handle);

    protected override void Dispose(bool dispose)
    {
        view.DestroyContext(handle);
    }
}
