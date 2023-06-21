// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Sekai.Desktop.Input;
using Sekai.Desktop.Windowing;
using Sekai.Framework.Input;
using Sekai.Framework.Windowing;

namespace Sekai.Desktop;

/// <summary>
/// A host that can run on desktop systems.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
public abstract class DesktopHost : Host, IInputContext
{
    public sealed override IEnumerable<IMonitor> Monitors => monitors.Values;

#pragma warning disable IDE0052

    private Silk.NET.GLFW.GlfwCallbacks.MonitorCallback? monitorCallback;
    private Silk.NET.GLFW.GlfwCallbacks.JoystickCallback? joystickCallback;

#pragma warning restore IDE0052

    private MergedInputContext? input;
    private readonly Lazy<Silk.NET.GLFW.Glfw> glfw = new(Silk.NET.GLFW.Glfw.GetApi);
    private readonly Dictionary<int, IGLFWController> devices = new();
    private readonly Dictionary<string, IMonitor> monitors = new();

    IEnumerable<IInputDevice> IInputContext.Devices => devices.Values;

    event Action<IInputDevice, bool>? IInputContext.ConnectionChanged
    {
        add => connectionChanged += value;
        remove => connectionChanged -= value;
    }

    private event Action<IInputDevice, bool>? connectionChanged;

    protected override unsafe void Initialize()
    {
        glfw.Value.InitHint(Silk.NET.GLFW.InitHint.JoystickHatButtons, false);

        if (!glfw.Value.Init())
        {
            throw new InvalidOperationException("Failed to initialize GLFW.");
        }

        glfw.Value.SetMonitorCallback(monitorCallback = handleMonitorChanged);
        glfw.Value.SetJoystickCallback(joystickCallback = handleControllersChanged);

        var ms = glfw.Value.GetMonitors(out int count);

        for (int i = 0; i < count; i++)
        {
            var m = createMonitorFromHandle(i, ms[i]);
            monitors.Add(m.Name, m);
        }

        base.Initialize();
    }

    protected override void Update()
    {
        base.Update();

        foreach (var device in devices.Values)
        {
            device.Update(glfw.Value);
        }
    }

    protected override void Shutdown()
    {
        base.Shutdown();
        input?.Dispose();
        glfw.Value.SetMonitorCallback(null);
        glfw.Value.SetJoystickCallback(null);
        glfw.Value.Terminate();
    }

    protected override IWindow CreateWindow()
    {
        return new Window(this, glfw.Value);
    }

    protected override IInputContext CreateInput(IWindow window)
    {
        return input = new MergedInputContext(this, (IInputContext)window);
    }

    private void handleControllersChanged(int id, Silk.NET.GLFW.ConnectedState state)
    {
        if (state is Silk.NET.GLFW.ConnectedState.Connected)
        {
            IGLFWController controller;

            if (glfw.Value.JoystickIsGamepad(id))
            {
                controller = new Gamepad(id, glfw.Value.GetGamepadName(id));
            }
            else
            {
                controller = new Joystick(id, glfw.Value.GetJoystickName(id));
            }

            devices.Add(id, controller);
            connectionChanged?.Invoke(controller, true);
        }
        else
        {
            if (devices.Remove(id, out var controller))
            {
                connectionChanged?.Invoke(controller, false);
            }
        }
    }

    private unsafe void handleMonitorChanged(Silk.NET.GLFW.Monitor* m, Silk.NET.GLFW.ConnectedState state)
    {
        Monitor monitor = default;

        if (state is Silk.NET.GLFW.ConnectedState.Connected)
        {
            monitor = createMonitorFromHandle(monitors.Count, m);
            monitors.Add(monitor.Name, monitor);
        }
        else
        {
            monitors.Remove(glfw.Value.GetMonitorName(m));
        }
    }

    private unsafe Monitor createMonitorFromHandle(int index, Silk.NET.GLFW.Monitor* monitor)
    {
        glfw.Value.GetMonitorPos(monitor, out int x, out int y);

        var modes = glfw.Value.GetVideoModes(monitor, out int modeCount);
        var ms = new VideoMode[modeCount];
        var mc = glfw.Value.GetVideoMode(monitor);

        for (int i = 0; i < modeCount; i++)
        {
            ms[i] = new(new(modes[i].Width, modes[i].Height), modes[i].RefreshRate);
        }

        string name = glfw.Value.GetMonitorName(monitor);

        return new(index, name, monitor, new(x, y), new(new(mc->Width, mc->Height), mc->RefreshRate), ms);
    }
}
