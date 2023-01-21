// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Windowing.OpenGL;

namespace Sekai.SDL;

internal class SDLGLContext : FrameworkObject, IOpenGLContext
{
    private readonly nint handle;
    private readonly SDLSurface view;

    public SDLGLContext(SDLSurface view, nint handle)
    {
        this.view = view;
        this.handle = handle;
    }

    public void MakeCurrent() => view.SetCurrentContext(handle);

    protected override void Destroy() => view.DestroyContext(handle);
}
