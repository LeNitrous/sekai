// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Sekai.Desktop.Input;
using Sekai.Desktop.Windowing;
using Sekai.Input;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai.Desktop;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
internal sealed unsafe class DesktopPlatform : Platform, IInputSource
{
    public override IMonitor PrimaryMonitor => monitors.Values.FirstOrDefault(m => m.Handle == glfw.GetPrimaryMonitor());
    public override IEnumerable<IMonitor> Monitors => monitors.Values.Cast<IMonitor>();

    private bool isDisposed;
    private event Action<IInputDevice, bool>? connectionChanged;
    private readonly Silk.NET.GLFW.Glfw glfw = Silk.NET.GLFW.Glfw.GetApi();
    private readonly Dictionary<string, Monitor> monitors = new();
    private readonly Dictionary<int, IGLFWController> devices = new();
    private readonly Silk.NET.GLFW.GlfwCallbacks.MonitorCallback? monitorCallback;
    private readonly Silk.NET.GLFW.GlfwCallbacks.JoystickCallback? joystickCallback;

    public DesktopPlatform(HostOptions options)
        : base(options)
    {
        glfw.InitHint(Silk.NET.GLFW.InitHint.JoystickHatButtons, false);

        if (!glfw.Init())
        {
            throw new InvalidOperationException("Failed to initialize GLFW.");
        }

        glfw.SetMonitorCallback(monitorCallback = handleMonitorChanged);
        glfw.SetJoystickCallback(joystickCallback = handleControllersChanged);

        var ms = glfw.GetMonitors(out int count);

        for (int i = 0; i < count; i++)
        {
            var m = Monitor.From(i, glfw, ms[i]);
            monitors.Add(m.Name, m);
        }
    }

    public override void DoEvents()
    {
        foreach (var device in devices.Values)
        {
            device.Update(glfw);
        }
    }

    public override unsafe void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        glfw.SetMonitorCallback(null);
        glfw.SetJoystickCallback(null);
        glfw.Terminate();

        isDisposed = true;
    }

    public override IWindow CreateWindow()
    {
        return new Window(glfw, Options.Name);
    }

    public override Storage CreateStorage()
    {
        var storage = new MountableStorage();

        storage.Mount(Storage.Game, new NativeStorage(AppDomain.CurrentDomain.BaseDirectory), false);
        storage.Mount(Storage.User, new NativeStorage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Options.Name)));
        storage.Mount(Storage.Temp, new NativeStorage(Path.GetTempPath()));

        return storage;
    }

    private void handleControllersChanged(int id, Silk.NET.GLFW.ConnectedState state)
    {
        if (state is Silk.NET.GLFW.ConnectedState.Connected)
        {
            IGLFWController controller;

            if (glfw.JoystickIsGamepad(id))
            {
                controller = new Gamepad(id, glfw.GetGamepadName(id));
            }
            else
            {
                controller = new Joystick(id, glfw.GetJoystickName(id));
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

    private void handleMonitorChanged(Silk.NET.GLFW.Monitor* m, Silk.NET.GLFW.ConnectedState state)
    {
        Monitor monitor = default;

        if (state is Silk.NET.GLFW.ConnectedState.Connected)
        {
            monitor = Monitor.From(monitors.Count, glfw, m);
            monitors.Add(monitor.Name, monitor);
        }
        else
        {
            monitors.Remove(glfw.GetMonitorName(m));
        }
    }

    IEnumerable<IInputDevice> IInputSource.Devices => devices.Values;

    event Action<IInputDevice, bool>? IInputSource.ConnectionChanged
    {
        add => connectionChanged += value;
        remove => connectionChanged -= value;
    }
}
