// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal unsafe class SDLView : FrameworkObject, IView, INativeWindowSource, IOpenGLContextSource
{
    public INativeWindow Native => native.Value;
    public IOpenGLContext GL => context.Value;
    public bool Active { get; private set; } = true;

    public event Action? OnClose;
    public event Func<bool>? OnCloseRequested;
    public event Action<bool>? OnStateChanged;
    internal event Action<Event>? OnProcessEvent;

#pragma warning disable IDE0052

    // References must be kept to avoid being garbage collected.
    private readonly EventFilter filter;

#pragma warning restore IDE0052

    public Size2 Size { get; private set; }
    public Mathematics.Point Position { get; private set; }

    internal readonly Window* Window;
    internal readonly Sdl Sdl;

    private readonly Lazy<INativeWindow> native;
    private readonly Lazy<IOpenGLContext> context;

    public SDLView()
    {
        Sdl = Sdl.GetApi();

        if (Sdl.Init(Sdl.InitVideo | Sdl.InitGamecontroller) != 0)
            Sdl.ThrowError();

        if (Sdl.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Core) != 0)
            Sdl.ThrowError();

        if (Sdl.GLSetAttribute(GLattr.ContextMajorVersion, 3) != 0)
            Sdl.ThrowError();

        if (Sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3) != 0)
            Sdl.ThrowError();

        if (Sdl.GLSetAttribute(GLattr.ContextFlags, (int)GLcontextFlag.DebugFlag) != 0)
            Sdl.ThrowError();

        Window = Sdl.CreateWindow("Sekai", Sdl.WindowposCentered, Sdl.WindowposCentered, 1280, 720, (uint)(WindowFlags.Hidden | WindowFlags.Opengl));

        native = new(() => new SDLNativeWindow(this));
        context = new(() => new SDLGLContext(this));

        Sdl.SetEventFilter(filter = handleSdlEvent, null);

        int w = 0;
        int h = 0;
        Sdl.GetWindowSize(Window, ref w, ref h);
        Size = new Size2(w, h);

        int x = 0;
        int y = 0;
        Sdl.GetWindowPosition(Window, ref x, ref y);
        Position = new Mathematics.Point(x, y);
    }

    private const int events_per_peep = 64;
    private readonly Event[] events = new Event[events_per_peep];

    public void DoEvents()
    {
        Sdl.PumpEvents();

        int eventsRead;

        do
        {
            fixed (Event* ptr = &events[0])
                eventsRead = Sdl.PeepEvents(ptr, events_per_peep, Eventaction.Getevent, (uint)EventType.Firstevent, (uint)EventType.Lastevent);

            for (int i = 0; i < eventsRead; i++)
            {
                ProcessEvent(events[i]);
            }

        } while (eventsRead == events_per_peep);
    }

    public Mathematics.Point PointToClient(Mathematics.Point point)
    {
        int x = 0;
        int y = 0;
        Sdl.GetWindowPosition(Window, ref x, ref y);
        return new Mathematics.Point(point.X - x, point.Y - y);
    }

    public Mathematics.Point PointToScreen(Mathematics.Point point)
    {
        return new Mathematics.Point(point.X + Position.X, point.Y + Position.Y);
    }

    protected virtual void ProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Quit:
            case EventType.AppTerminating:
                handleQuitEvent();
                break;

            case EventType.Windowevent:
                handleWindowEvent(evt.Window);
                break;
        }

        OnProcessEvent?.Invoke(evt);
    }

    private void handleQuitEvent()
    {
        if (OnCloseRequested?.Invoke() ?? true)
            Close();
    }

    private void handleWindowEvent(WindowEvent evt)
    {
        var wtype = (WindowEventID)evt.Event;

        switch (wtype)
        {
            case WindowEventID.Resized:
                Size = new Size2(evt.Data1, evt.Data2);
                break;

            case WindowEventID.Moved:
                Position = new Mathematics.Point(evt.Data1, evt.Data2);
                break;
        }
    }

    private int handleSdlEvent(void* data, Event* evt)
    {
        var type = (EventType)evt->Type;

        switch (type)
        {
            case EventType.AppDidenterbackground:
                {
                    Active = false;
                    OnStateChanged?.Invoke(Active);
                    break;
                }

            case EventType.AppDidenterforeground:
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

    protected override void Destroy()
    {
        Sdl.DestroyWindow(Window);
        Sdl.Quit();
        Sdl.Dispose();
    }
}
