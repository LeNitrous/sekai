// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Sekai.Framework;
using Sekai.Framework.Windowing;
using Sekai.Framework.Windowing.OpenGL;
using static SDL2.SDL;

namespace Sekai.SDL;

internal class SDLView : FrameworkObject, IView, INativeWindowSource, IOpenGLProviderSource
{
    public INativeWindow Native { get; }
    public bool Active { get; private set; } = true;

    public event Action OnClose = null!;
    public event Func<bool> OnCloseRequested = null!;
    public event Action<bool> OnStateChanged = null!;

    internal event Action<SDL_Event> OnProcessEvent = null!;

#pragma warning disable IDE0052

    // References must be kept to avoid being garbage collected.
    private readonly SDL_EventFilter filter;

#pragma warning restore IDE0052

    public Size Size
    {
        get
        {
            SDL_GetWindowSize(Window, out int w, out int h);
            return new Size(w, h);
        }
    }

    internal readonly nint Window;

    public SDLView()
    {
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMECONTROLLER) < 0)
        {
            throw new InvalidOperationException($"Failed to initialize SDL: {SDL_GetError()}");
        }

        if (SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE) != 0)
            throw new InvalidOperationException(SDL_GetError());

        if (SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3) != 0)
            throw new InvalidOperationException(SDL_GetError());

        if (SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3) != 0)
            throw new InvalidOperationException(SDL_GetError());

        Window = SDL_CreateWindow("Sekai", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1280, 720, SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_OPENGL);
        Native = new SDLNativeWindow(this);

        SDL_SetEventFilter(filter = handleSdlEvent, IntPtr.Zero);
    }

    private const int events_per_peep = 64;
    private readonly SDL_Event[] events = new SDL_Event[events_per_peep];

    public void DoEvents()
    {
        SDL_PumpEvents();

        int eventsRead;

        do
        {
            eventsRead = SDL_PeepEvents(events, events_per_peep, SDL_eventaction.SDL_GETEVENT, SDL_EventType.SDL_FIRSTEVENT, SDL_EventType.SDL_LASTEVENT);

            for (int i = 0; i < eventsRead; i++)
            {
                ProcessEvent(events[i]);
            }

        } while (eventsRead == events_per_peep);
    }

    public Point PointToClient(Point point)
    {
        SDL_GetWindowPosition(Window, out int x, out int y);
        return new Point(point.X - x, point.Y - y);
    }

    public Point PointToScreen(Point point)
    {
        SDL_GetWindowPosition(Window, out int x, out int y);
        return new Point(point.X + x, point.Y + y);
    }

    protected virtual void ProcessEvent(SDL_Event evt)
    {
        switch (evt.type)
        {
            case SDL_EventType.SDL_QUIT:
            case SDL_EventType.SDL_APP_TERMINATING:
                handleQuitEvent();
                break;
        }

        OnProcessEvent?.Invoke(evt);
    }

    private void handleQuitEvent()
    {
        if (OnCloseRequested?.Invoke() ?? true)
            Close();
    }

    private int handleSdlEvent(IntPtr userData, IntPtr ptr)
    {
        var evt = Marshal.PtrToStructure<SDL_Event>(ptr);

        switch (evt.type)
        {
            case SDL_EventType.SDL_APP_DIDENTERBACKGROUND:
                {
                    Active = false;
                    OnStateChanged?.Invoke(Active);
                    break;
                }

            case SDL_EventType.SDL_APP_DIDENTERFOREGROUND:
                {
                    Active = true;
                    OnStateChanged?.Invoke(Active);
                    break;
                }
        }

        return 1;
    }

    public void Close()
    {
        OnClose?.Invoke();
    }

    private IOpenGLProvider gl = null!;
    public IOpenGLProvider GL => gl ??= new SDLGLProvider(this);

    protected override void Destroy()
    {
        if (Window != IntPtr.Zero)
            SDL_DestroyWindow(Window);

        if (gl is not null)
            gl.Dispose();

        SDL_Quit();
    }
}
