// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sekai.Mathematics;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal unsafe class SDLSurface : Windowing.Surface, INativeWindowSource, IOpenGLContextSource
{
    public override string Name { get; } = @"SDL";
    public override System.Version Version { get; }
    public INativeWindow Native => native.Value;
    public override bool Active => active;
    public override event Action<bool>? OnStateChanged;
    public override event Action? OnClose;
    public override event Action? OnUpdate;
    public override event Func<bool>? OnCloseRequested;

    internal event Action<Event>? OnProcessEvent;

#pragma warning disable IDE0052

    // References must be kept to avoid being garbage collected.
    private readonly EventFilter filter;

#pragma warning restore IDE0052

    public override Size2 Size => size;
    public override Mathematics.Point Position => position;

    internal readonly Window* Window;
    internal readonly Sdl Sdl;

    private bool alive;
    private bool active;
    private Size2 size;
    private Mathematics.Point position;
    private readonly Lazy<INativeWindow> native;
    private readonly ConcurrentQueue<Action> queue = new();

    public SDLSurface()
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

        Sdl.SetEventFilter(filter = filterSdlEvent, null);

        Silk.NET.SDL.Version version = new();
        Sdl.GetVersion(ref version);
        Version = new(version.Major, version.Minor, version.Patch);

        updateWindowSize();
        updateWindowPosition();
    }

    private const int events_per_peep = 64;
    private readonly Event[] events = new Event[events_per_peep];

    public override void Run()
    {
        if (alive)
            return;

        alive = true;

        while (alive)
        {
            while (queue.TryDequeue(out var action))
                action();

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

            OnUpdate?.Invoke();
        }

        Sdl.DestroyWindow(Window);
        Sdl.Quit();
        Sdl.Dispose();
    }

    public override Mathematics.Point PointToClient(Mathematics.Point point)
    {
        int x = 0;
        int y = 0;
        Sdl.GetWindowPosition(Window, ref x, ref y);
        return new Mathematics.Point(point.X - x, point.Y - y);
    }

    public override Mathematics.Point PointToScreen(Mathematics.Point point)
    {
        return new Mathematics.Point(point.X + Position.X, point.Y + Position.Y);
    }

    internal void Invoke(Action action) => queue.Enqueue(action);

    protected virtual void ProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Windowevent:
                handleWindowEvent(evt.Window);
                break;

            case EventType.AppDidenterbackground:
                OnStateChanged?.Invoke(active = false);
                break;

            case EventType.AppDidenterforeground:
                OnStateChanged?.Invoke(active = true);
                break;
            
            case EventType.Quit:
            case EventType.AppTerminating:
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

    private void handleWindowEvent(WindowEvent evt)
    {
        var wtype = (WindowEventID)evt.Event;

        switch (wtype)
        {
            case WindowEventID.Resized:
                updateWindowSize();
                break;

            case WindowEventID.Moved:
                updateWindowPosition();
                break;
        }
    }

    private void updateWindowSize()
    {
        int w = 0;
        int h = 0;
        Sdl.GetWindowSize(Window, ref w, ref h);
        size = new Size2(w, h);
    }

    private void updateWindowPosition()
    {
        int x = 0;
        int y = 0;
        Sdl.GetWindowPosition(Window, ref x, ref y);
        position = new Mathematics.Point(x, y);
    }

    private int filterSdlEvent(void* data, Event* evt)
    {
        if (evt->Type == (uint)EventType.Windowevent && evt->Window.Event == (byte)WindowEventID.Resized)
        {
            ProcessEvent(Unsafe.Read<Event>(evt));
            return 0;
        }

        return 1;
    }

    public override void Close() => Invoke(() =>
    {
        alive = false;
        OnClose?.Invoke();
    });

    private readonly Dictionary<nint, IOpenGLContext> contexts = new();

    public nint GetProcAddress(string name)
    {
        return (nint)Sdl.GLGetProcAddress(name);
    }

    public void ClearCurrentContext()
    {
        if (Sdl.GLMakeCurrent(null, null) != 0)
            Sdl.ThrowError();
    }

    public void SwapBuffers()
    {
        Sdl.GLSwapWindow(Window);
    }

    public void SetSyncToVerticalBlank(bool sync)
    {
        if (Sdl.GLSetSwapInterval(sync ? 1 : 0) != 0)
            Sdl.ThrowError();
    }

    public unsafe IOpenGLContext CreateContext()
    {
        nint handle = (nint)Sdl.GLCreateContext(Window);

        if (handle == IntPtr.Zero)
            Sdl.ThrowError();

        var context = new SDLGLContext(this, handle);

        contexts.Add(handle, context);

        return context;
    }

    public IOpenGLContext? GetCurrentContext()
    {
        nint handle = (nint)Sdl.GLGetCurrentContext();

        if (handle == IntPtr.Zero)
            return null;

        if (!contexts.TryGetValue(handle, out var context))
            throw new InvalidOperationException(@"Attempting to get context of an unknown pointer.");

        return context;
    }

    internal void SetCurrentContext(nint handle)
    {
        if (Sdl.GLMakeCurrent(Window, (void*)handle) != 0)
            Sdl.ThrowError();
    }

    internal void DestroyContext(nint handle)
    {
        if (contexts.Remove(handle))
            Sdl.GLDeleteContext((void*)handle);
    }
}
