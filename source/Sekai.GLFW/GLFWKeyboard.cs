// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;

namespace Sekai.GLFW;

internal sealed unsafe class GLFWKeyboard : IKeyboard
{
    public IReadOnlyList<Key> Keys => keys;
    public string Name => name;
    public bool IsConnected => true;
    public event Action<IKeyboard, Key, bool>? OnKey;

#pragma warning disable IDE0060

    public void HandleKeyboardKey(Silk.NET.GLFW.WindowHandle* window, Silk.NET.GLFW.Keys key, int scancode, Silk.NET.GLFW.InputAction action, Silk.NET.GLFW.KeyModifiers mods)
    {
        switch (action)
        {
            case Silk.NET.GLFW.InputAction.Press:
                OnKey?.Invoke(this, toKey(key), true);
                break;

            case Silk.NET.GLFW.InputAction.Release:
                OnKey?.Invoke(this, toKey(key), false);
                break;

            default:
                break;
        }
    }

#pragma warning restore IDE0060

    private const string name = "Keyboard";

    private static readonly Key[] keys = Enum.GetValues<Key>();

    private static Key toKey(Silk.NET.GLFW.Keys key)
    {
        switch (key)
        {
            case Silk.NET.GLFW.Keys.Space:
                return Key.Space;

            case Silk.NET.GLFW.Keys.Apostrophe:
                return Key.Apostrophe;

            case Silk.NET.GLFW.Keys.Comma:
                return Key.Comma;

            case Silk.NET.GLFW.Keys.Minus:
                return Key.Minus;

            case Silk.NET.GLFW.Keys.Period:
                return Key.Period;

            case Silk.NET.GLFW.Keys.Slash:
                return Key.Slash;

            case Silk.NET.GLFW.Keys.Number0:
                return Key.Number0;

            case Silk.NET.GLFW.Keys.Number1:
                return Key.Number1;

            case Silk.NET.GLFW.Keys.Number2:
                return Key.Number2;

            case Silk.NET.GLFW.Keys.Number3:
                return Key.Number3;

            case Silk.NET.GLFW.Keys.Number4:
                return Key.Number4;

            case Silk.NET.GLFW.Keys.Number5:
                return Key.Number5;

            case Silk.NET.GLFW.Keys.Number6:
                return Key.Number6;

            case Silk.NET.GLFW.Keys.Number7:
                return Key.Number7;

            case Silk.NET.GLFW.Keys.Number8:
                return Key.Number8;

            case Silk.NET.GLFW.Keys.Number9:
                return Key.Number9;

            case Silk.NET.GLFW.Keys.Semicolon:
                return Key.Semicolon;

            case Silk.NET.GLFW.Keys.Equal:
                return Key.Equal;

            case Silk.NET.GLFW.Keys.A:
                return Key.A;

            case Silk.NET.GLFW.Keys.B:
                return Key.B;

            case Silk.NET.GLFW.Keys.C:
                return Key.C;

            case Silk.NET.GLFW.Keys.D:
                return Key.D;

            case Silk.NET.GLFW.Keys.E:
                return Key.E;

            case Silk.NET.GLFW.Keys.F:
                return Key.F;

            case Silk.NET.GLFW.Keys.G:
                return Key.G;

            case Silk.NET.GLFW.Keys.H:
                return Key.H;

            case Silk.NET.GLFW.Keys.I:
                return Key.I;

            case Silk.NET.GLFW.Keys.J:
                return Key.J;

            case Silk.NET.GLFW.Keys.K:
                return Key.K;

            case Silk.NET.GLFW.Keys.L:
                return Key.L;

            case Silk.NET.GLFW.Keys.M:
                return Key.M;

            case Silk.NET.GLFW.Keys.N:
                return Key.N;

            case Silk.NET.GLFW.Keys.O:
                return Key.O;

            case Silk.NET.GLFW.Keys.P:
                return Key.P;

            case Silk.NET.GLFW.Keys.Q:
                return Key.Q;

            case Silk.NET.GLFW.Keys.R:
                return Key.R;

            case Silk.NET.GLFW.Keys.S:
                return Key.S;

            case Silk.NET.GLFW.Keys.T:
                return Key.T;

            case Silk.NET.GLFW.Keys.U:
                return Key.U;

            case Silk.NET.GLFW.Keys.V:
                return Key.V;

            case Silk.NET.GLFW.Keys.W:
                return Key.W;

            case Silk.NET.GLFW.Keys.X:
                return Key.X;

            case Silk.NET.GLFW.Keys.Y:
                return Key.Y;

            case Silk.NET.GLFW.Keys.Z:
                return Key.Z;

            case Silk.NET.GLFW.Keys.LeftBracket:
                return Key.LeftBracket;

            case Silk.NET.GLFW.Keys.BackSlash:
                return Key.Backslash;

            case Silk.NET.GLFW.Keys.RightBracket:
                return Key.RightBracket;

            case Silk.NET.GLFW.Keys.GraveAccent:
                return Key.GraveAccent;

            case Silk.NET.GLFW.Keys.World1:
                return Key.World1;

            case Silk.NET.GLFW.Keys.World2:
                return Key.World2;

            case Silk.NET.GLFW.Keys.Escape:
                return Key.Escape;

            case Silk.NET.GLFW.Keys.Enter:
                return Key.Enter;

            case Silk.NET.GLFW.Keys.Tab:
                return Key.Tab;

            case Silk.NET.GLFW.Keys.Backspace:
                return Key.Backspace;

            case Silk.NET.GLFW.Keys.Insert:
                return Key.Insert;

            case Silk.NET.GLFW.Keys.Delete:
                return Key.Delete;

            case Silk.NET.GLFW.Keys.Right:
                return Key.Right;

            case Silk.NET.GLFW.Keys.Left:
                return Key.Left;

            case Silk.NET.GLFW.Keys.Down:
                return Key.Down;

            case Silk.NET.GLFW.Keys.Up:
                return Key.Up;

            case Silk.NET.GLFW.Keys.PageUp:
                return Key.PageUp;

            case Silk.NET.GLFW.Keys.PageDown:
                return Key.PageDown;

            case Silk.NET.GLFW.Keys.Home:
                return Key.Home;

            case Silk.NET.GLFW.Keys.End:
                return Key.End;

            case Silk.NET.GLFW.Keys.CapsLock:
                return Key.CapsLock;

            case Silk.NET.GLFW.Keys.ScrollLock:
                return Key.ScrollLock;

            case Silk.NET.GLFW.Keys.NumLock:
                return Key.NumLock;

            case Silk.NET.GLFW.Keys.PrintScreen:
                return Key.PrintScreen;

            case Silk.NET.GLFW.Keys.Pause:
                return Key.Pause;

            case Silk.NET.GLFW.Keys.F1:
                return Key.F1;

            case Silk.NET.GLFW.Keys.F2:
                return Key.F2;

            case Silk.NET.GLFW.Keys.F3:
                return Key.F3;

            case Silk.NET.GLFW.Keys.F4:
                return Key.F4;

            case Silk.NET.GLFW.Keys.F5:
                return Key.F5;

            case Silk.NET.GLFW.Keys.F6:
                return Key.F6;

            case Silk.NET.GLFW.Keys.F7:
                return Key.F7;

            case Silk.NET.GLFW.Keys.F8:
                return Key.F8;

            case Silk.NET.GLFW.Keys.F9:
                return Key.F9;

            case Silk.NET.GLFW.Keys.F10:
                return Key.F10;

            case Silk.NET.GLFW.Keys.F11:
                return Key.F11;

            case Silk.NET.GLFW.Keys.F12:
                return Key.F12;

            case Silk.NET.GLFW.Keys.F13:
                return Key.F13;

            case Silk.NET.GLFW.Keys.F14:
                return Key.F14;

            case Silk.NET.GLFW.Keys.F15:
                return Key.F15;

            case Silk.NET.GLFW.Keys.F16:
                return Key.F16;

            case Silk.NET.GLFW.Keys.F17:
                return Key.F17;

            case Silk.NET.GLFW.Keys.F18:
                return Key.F18;

            case Silk.NET.GLFW.Keys.F19:
                return Key.F19;

            case Silk.NET.GLFW.Keys.F20:
                return Key.F20;

            case Silk.NET.GLFW.Keys.F21:
                return Key.F21;

            case Silk.NET.GLFW.Keys.F22:
                return Key.F22;

            case Silk.NET.GLFW.Keys.F23:
                return Key.F23;

            case Silk.NET.GLFW.Keys.F24:
                return Key.F24;

            case Silk.NET.GLFW.Keys.F25:
                return Key.F25;

            case Silk.NET.GLFW.Keys.Keypad0:
                return Key.Keypad0;

            case Silk.NET.GLFW.Keys.Keypad1:
                return Key.Keypad1;

            case Silk.NET.GLFW.Keys.Keypad2:
                return Key.Keypad2;

            case Silk.NET.GLFW.Keys.Keypad3:
                return Key.Keypad3;

            case Silk.NET.GLFW.Keys.Keypad4:
                return Key.Keypad4;

            case Silk.NET.GLFW.Keys.Keypad5:
                return Key.Keypad5;

            case Silk.NET.GLFW.Keys.Keypad6:
                return Key.Keypad6;

            case Silk.NET.GLFW.Keys.Keypad7:
                return Key.Keypad7;

            case Silk.NET.GLFW.Keys.Keypad8:
                return Key.Keypad8;

            case Silk.NET.GLFW.Keys.Keypad9:
                return Key.Keypad9;

            case Silk.NET.GLFW.Keys.KeypadDecimal:
                return Key.KeypadDecimal;

            case Silk.NET.GLFW.Keys.KeypadDivide:
                return Key.KeypadDivide;

            case Silk.NET.GLFW.Keys.KeypadMultiply:
                return Key.KeypadMultiply;

            case Silk.NET.GLFW.Keys.KeypadSubtract:
                return Key.KeypadMinus;

            case Silk.NET.GLFW.Keys.KeypadAdd:
                return Key.KeypadAdd;

            case Silk.NET.GLFW.Keys.KeypadEnter:
                return Key.KeypadEnter;

            case Silk.NET.GLFW.Keys.KeypadEqual:
                return Key.KeypadEqual;

            case Silk.NET.GLFW.Keys.ShiftLeft:
                return Key.LeftShift;

            case Silk.NET.GLFW.Keys.ControlLeft:
                return Key.LeftControl;

            case Silk.NET.GLFW.Keys.AltLeft:
                return Key.LeftAlt;

            case Silk.NET.GLFW.Keys.SuperLeft:
                return Key.LeftSuper;

            case Silk.NET.GLFW.Keys.ShiftRight:
                return Key.RightShift;

            case Silk.NET.GLFW.Keys.ControlRight:
                return Key.RightControl;

            case Silk.NET.GLFW.Keys.AltRight:
                return Key.RightAlt;

            case Silk.NET.GLFW.Keys.SuperRight:
                return Key.RightSuper;

            case Silk.NET.GLFW.Keys.Menu:
                return Key.Menu;

            default:
                throw new ArgumentOutOfRangeException(nameof(key));
        }
    }
}
