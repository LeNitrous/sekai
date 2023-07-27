// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Input;

namespace Sekai.GLFW;

internal abstract class GLFWController : IInputDevice
{
    public int Index { get; }
    public Deadzone Deadzone { get; set; }
    public string Name => Glfw.GetGamepadName(Index);
    public abstract bool IsConnected { get; }
    protected Silk.NET.GLFW.Glfw Glfw { get; private set; }

    protected GLFWController(Silk.NET.GLFW.Glfw glfw, int index)
    {
        Glfw = glfw;
        Index = index;
    }

    public abstract void Update();
}
