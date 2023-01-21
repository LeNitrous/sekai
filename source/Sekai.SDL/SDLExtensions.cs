// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Keyboards;
using Sekai.Windowing;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal static class SDLExtensions
{
    public static Key ToKey(this Keysym key)
    {
        bool numLockOn = ((Keymod)key.Mod).HasFlag(Keymod.Num) || RuntimeInfo.IsApple;

        return key.Scancode switch
        {
            Scancode.ScancodeA => Key.A,
            Scancode.ScancodeB => Key.B,
            Scancode.ScancodeC => Key.C,
            Scancode.ScancodeD => Key.D,
            Scancode.ScancodeE => Key.E,
            Scancode.ScancodeF => Key.F,
            Scancode.ScancodeG => Key.G,
            Scancode.ScancodeH => Key.H,
            Scancode.ScancodeI => Key.I,
            Scancode.ScancodeJ => Key.J,
            Scancode.ScancodeK => Key.K,
            Scancode.ScancodeL => Key.L,
            Scancode.ScancodeM => Key.M,
            Scancode.ScancodeN => Key.N,
            Scancode.ScancodeO => Key.O,
            Scancode.ScancodeP => Key.P,
            Scancode.ScancodeQ => Key.Q,
            Scancode.ScancodeR => Key.R,
            Scancode.ScancodeS => Key.S,
            Scancode.ScancodeT => Key.T,
            Scancode.ScancodeU => Key.U,
            Scancode.ScancodeV => Key.V,
            Scancode.ScancodeW => Key.W,
            Scancode.ScancodeX => Key.X,
            Scancode.ScancodeY => Key.Y,
            Scancode.ScancodeZ => Key.Z,
            Scancode.Scancode1 => Key.Number1,
            Scancode.Scancode2 => Key.Number2,
            Scancode.Scancode3 => Key.Number3,
            Scancode.Scancode4 => Key.Number4,
            Scancode.Scancode5 => Key.Number5,
            Scancode.Scancode6 => Key.Number6,
            Scancode.Scancode7 => Key.Number7,
            Scancode.Scancode8 => Key.Number8,
            Scancode.Scancode9 => Key.Number9,
            Scancode.Scancode0 => Key.Number0,
            Scancode.ScancodeReturn => Key.Enter,
            Scancode.ScancodeEscape => Key.Escape,
            Scancode.ScancodeBackspace => Key.Backspace,
            Scancode.ScancodeTab => Key.Tab,
            Scancode.ScancodeSpace => Key.Space,
            Scancode.ScancodeMinus => Key.Minus,
            Scancode.ScancodeEquals => Key.Equal,
            Scancode.ScancodeLeftbracket => Key.LeftBracket,
            Scancode.ScancodeRightbracket => Key.RightBracket,
            Scancode.ScancodeBackslash => Key.BackSlash,
            Scancode.ScancodeSemicolon => Key.Semicolon,
            Scancode.ScancodeApostrophe => Key.Apostrophe,
            Scancode.ScancodeGrave => Key.GraveAccent,
            Scancode.ScancodeComma => Key.Comma,
            Scancode.ScancodePeriod => Key.Period,
            Scancode.ScancodeSlash => Key.Slash,
            Scancode.ScancodeCapslock => Key.CapsLock,
            Scancode.ScancodeF1 => Key.F1,
            Scancode.ScancodeF2 => Key.F2,
            Scancode.ScancodeF3 => Key.F3,
            Scancode.ScancodeF4 => Key.F4,
            Scancode.ScancodeF5 => Key.F5,
            Scancode.ScancodeF6 => Key.F6,
            Scancode.ScancodeF7 => Key.F7,
            Scancode.ScancodeF8 => Key.F8,
            Scancode.ScancodeF9 => Key.F9,
            Scancode.ScancodeF10 => Key.F10,
            Scancode.ScancodeF11 => Key.F11,
            Scancode.ScancodeF12 => Key.F12,
            Scancode.ScancodePrintscreen => Key.PrintScreen,
            Scancode.ScancodeScrolllock => Key.ScrollLock,
            Scancode.ScancodePause => Key.Pause,
            Scancode.ScancodeInsert => Key.Insert,
            Scancode.ScancodeHome => Key.Home,
            Scancode.ScancodePageup => Key.PageUp,
            Scancode.ScancodeDelete => Key.Delete,
            Scancode.ScancodeEnd => Key.End,
            Scancode.ScancodePagedown => Key.PageDown,
            Scancode.ScancodeRight => Key.Right,
            Scancode.ScancodeLeft => Key.Left,
            Scancode.ScancodeDown => Key.Down,
            Scancode.ScancodeUp => Key.Up,
            Scancode.ScancodeNumlockclear => Key.NumLock,
            Scancode.ScancodeKPDivide => Key.KeypadDivide,
            Scancode.ScancodeKPMultiply => Key.KeypadMultiply,
            Scancode.ScancodeKPMinus => Key.KeypadSubtract,
            Scancode.ScancodeKPPlus => Key.KeypadAdd,
            Scancode.ScancodeKPEnter => Key.KeypadEnter,
            Scancode.ScancodeKP1 => numLockOn ? Key.Keypad1 : Key.End,
            Scancode.ScancodeKP2 => numLockOn ? Key.Keypad2 : Key.Down,
            Scancode.ScancodeKP3 => numLockOn ? Key.Keypad3 : Key.PageDown,
            Scancode.ScancodeKP4 => numLockOn ? Key.Keypad4 : Key.Left,
            Scancode.ScancodeKP5 => Key.Keypad5,
            Scancode.ScancodeKP6 => numLockOn ? Key.Keypad6 : Key.Right,
            Scancode.ScancodeKP7 => numLockOn ? Key.Keypad7 : Key.Home,
            Scancode.ScancodeKP8 => numLockOn ? Key.Keypad8 : Key.Up,
            Scancode.ScancodeKP9 => numLockOn ? Key.Keypad9 : Key.PageUp,
            Scancode.ScancodeKP0 => numLockOn ? Key.Keypad0 : Key.Insert,
            Scancode.ScancodeKPPeriod => numLockOn ? Key.KeypadDecimal : Key.Delete,
            Scancode.ScancodeNonusbackslash => Key.BackSlash,
            Scancode.ScancodeMenu or Scancode.ScancodeApplication => Key.Menu,
            Scancode.ScancodeKPEquals => Key.KeypadEqual,
            Scancode.ScancodeF13 => Key.F13,
            Scancode.ScancodeF14 => Key.F14,
            Scancode.ScancodeF15 => Key.F15,
            Scancode.ScancodeF16 => Key.F16,
            Scancode.ScancodeF17 => Key.F17,
            Scancode.ScancodeF18 => Key.F18,
            Scancode.ScancodeF19 => Key.F19,
            Scancode.ScancodeF20 => Key.F20,
            Scancode.ScancodeF21 => Key.F21,
            Scancode.ScancodeF22 => Key.F22,
            Scancode.ScancodeF23 => Key.F23,
            Scancode.ScancodeF24 => Key.F24,
            Scancode.ScancodeDecimalseparator => Key.KeypadDecimal,
            Scancode.ScancodeLctrl => Key.ControlLeft,
            Scancode.ScancodeLshift => Key.ShiftLeft,
            Scancode.ScancodeLalt => Key.AltLeft,
            Scancode.ScancodeLgui => Key.SuperLeft,
            Scancode.ScancodeRctrl => Key.ControlRight,
            Scancode.ScancodeRshift => Key.ShiftRight,
            Scancode.ScancodeRalt => Key.AltRight,
            Scancode.ScancodeRgui => Key.SuperRight,
            _ => Key.Unknown,
        };
    }

    public static Scancode ToScancode(this Key key)
    {
        return key switch
        {
            Key.Space => Scancode.ScancodeSpace,
            Key.Apostrophe => Scancode.ScancodeApostrophe,
            Key.Comma => Scancode.ScancodeComma,
            Key.Minus => Scancode.ScancodeMinus,
            Key.Period => Scancode.ScancodePeriod,
            Key.Slash => Scancode.ScancodeSlash,
            Key.Number0 => Scancode.Scancode0,
            Key.Number1 => Scancode.Scancode1,
            Key.Number2 => Scancode.Scancode2,
            Key.Number3 => Scancode.Scancode3,
            Key.Number4 => Scancode.Scancode4,
            Key.Number5 => Scancode.Scancode5,
            Key.Number6 => Scancode.Scancode6,
            Key.Number7 => Scancode.Scancode7,
            Key.Number8 => Scancode.Scancode8,
            Key.Number9 => Scancode.Scancode9,
            Key.Semicolon => Scancode.ScancodeSemicolon,
            Key.Equal => Scancode.ScancodeEquals,
            Key.A => Scancode.ScancodeA,
            Key.B => Scancode.ScancodeB,
            Key.C => Scancode.ScancodeC,
            Key.D => Scancode.ScancodeD,
            Key.E => Scancode.ScancodeE,
            Key.F => Scancode.ScancodeF,
            Key.G => Scancode.ScancodeG,
            Key.H => Scancode.ScancodeH,
            Key.I => Scancode.ScancodeI,
            Key.J => Scancode.ScancodeJ,
            Key.K => Scancode.ScancodeK,
            Key.L => Scancode.ScancodeL,
            Key.M => Scancode.ScancodeM,
            Key.N => Scancode.ScancodeN,
            Key.O => Scancode.ScancodeO,
            Key.P => Scancode.ScancodeP,
            Key.Q => Scancode.ScancodeQ,
            Key.R => Scancode.ScancodeR,
            Key.S => Scancode.ScancodeS,
            Key.T => Scancode.ScancodeT,
            Key.U => Scancode.ScancodeU,
            Key.V => Scancode.ScancodeV,
            Key.W => Scancode.ScancodeW,
            Key.X => Scancode.ScancodeX,
            Key.Y => Scancode.ScancodeY,
            Key.Z => Scancode.ScancodeZ,
            Key.LeftBracket => Scancode.ScancodeLeftbracket,
            Key.BackSlash => Scancode.ScancodeBackslash,
            Key.RightBracket => Scancode.ScancodeRightbracket,
            Key.GraveAccent => Scancode.ScancodeGrave,
            Key.Escape => Scancode.ScancodeEscape,
            Key.Enter => Scancode.ScancodeReturn,
            Key.Tab => Scancode.ScancodeTab,
            Key.Backspace => Scancode.ScancodeBackspace,
            Key.Insert => Scancode.ScancodeInsert,
            Key.Delete => Scancode.ScancodeDelete,
            Key.Right => Scancode.ScancodeRight,
            Key.Left => Scancode.ScancodeLeft,
            Key.Down => Scancode.ScancodeDown,
            Key.Up => Scancode.ScancodeUp,
            Key.PageUp => Scancode.ScancodePageup,
            Key.PageDown => Scancode.ScancodePagedown,
            Key.Home => Scancode.ScancodeHome,
            Key.End => Scancode.ScancodeEnd,
            Key.CapsLock => Scancode.ScancodeCapslock,
            Key.ScrollLock => Scancode.ScancodeScrolllock,
            Key.NumLock => Scancode.ScancodeNumlockclear,
            Key.PrintScreen => Scancode.ScancodePrintscreen,
            Key.Pause => Scancode.ScancodePause,
            Key.F1 => Scancode.ScancodeF1,
            Key.F2 => Scancode.ScancodeF2,
            Key.F3 => Scancode.ScancodeF3,
            Key.F4 => Scancode.ScancodeF4,
            Key.F5 => Scancode.ScancodeF5,
            Key.F6 => Scancode.ScancodeF6,
            Key.F7 => Scancode.ScancodeF7,
            Key.F8 => Scancode.ScancodeF8,
            Key.F9 => Scancode.ScancodeF9,
            Key.F10 => Scancode.ScancodeF10,
            Key.F11 => Scancode.ScancodeF11,
            Key.F12 => Scancode.ScancodeF12,
            Key.F13 => Scancode.ScancodeF13,
            Key.F14 => Scancode.ScancodeF14,
            Key.F15 => Scancode.ScancodeF15,
            Key.F16 => Scancode.ScancodeF16,
            Key.F17 => Scancode.ScancodeF17,
            Key.F18 => Scancode.ScancodeF18,
            Key.F19 => Scancode.ScancodeF19,
            Key.F20 => Scancode.ScancodeF20,
            Key.F21 => Scancode.ScancodeF21,
            Key.F22 => Scancode.ScancodeF22,
            Key.F23 => Scancode.ScancodeF23,
            Key.F24 => Scancode.ScancodeF24,
            Key.Keypad0 => Scancode.ScancodeKP0,
            Key.Keypad1 => Scancode.ScancodeKP1,
            Key.Keypad2 => Scancode.ScancodeKP2,
            Key.Keypad3 => Scancode.ScancodeKP3,
            Key.Keypad4 => Scancode.ScancodeKP4,
            Key.Keypad5 => Scancode.ScancodeKP5,
            Key.Keypad6 => Scancode.ScancodeKP6,
            Key.Keypad7 => Scancode.ScancodeKP7,
            Key.Keypad8 => Scancode.ScancodeKP8,
            Key.Keypad9 => Scancode.ScancodeKP9,
            Key.KeypadDecimal => Scancode.ScancodeKPDecimal,
            Key.KeypadDivide => Scancode.ScancodeKPDivide,
            Key.KeypadMultiply => Scancode.ScancodeKPMultiply,
            Key.KeypadSubtract => Scancode.ScancodeKPMinus,
            Key.KeypadAdd => Scancode.ScancodeKPPlus,
            Key.KeypadEnter => Scancode.ScancodeKPEnter,
            Key.KeypadEqual => Scancode.ScancodeKPEquals,
            Key.ShiftLeft => Scancode.ScancodeLshift,
            Key.ControlLeft => Scancode.ScancodeLctrl,
            Key.AltLeft => Scancode.ScancodeLalt,
            Key.SuperLeft => Scancode.ScancodeLgui,
            Key.ShiftRight => Scancode.ScancodeRshift,
            Key.ControlRight => Scancode.ScancodeRctrl,
            Key.AltRight => Scancode.ScancodeRalt,
            Key.SuperRight => Scancode.ScancodeRgui,
            Key.Menu => Scancode.ScancodeMenu,
            _ => Scancode.ScancodeUnknown,
        };
    }

    public static WindowState ToWindowState(this WindowFlags flags)
    {
        if ((flags & (WindowFlags.Fullscreen | WindowFlags.FullscreenDesktop)) != 0)
        {
            return WindowState.Fullscreen;
        }

        if ((flags & WindowFlags.Maximized) != 0)
        {
            return WindowState.Maximized;
        }

        if ((flags & WindowFlags.Minimized) != 0)
        {
            return WindowState.Minimized;
        }

        return WindowState.Normal;
    }

    public static WindowBorder ToWindowBorder(this WindowFlags flags)
    {
        if ((flags & WindowFlags.Resizable) != 0)
        {
            return WindowBorder.Resizable;
        }

        return (flags & WindowFlags.Borderless) != 0 ? WindowBorder.Hidden : WindowBorder.Fixed;
    }
}
