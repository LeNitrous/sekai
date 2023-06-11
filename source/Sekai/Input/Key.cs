// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

/// <summary>
/// Keyboard input key.
/// </summary>
public enum Key
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// Space.
    /// </summary>
    Space = 32,

    /// <summary>
    /// Apostrophe.
    /// </summary>
    Apostrophe = 39,

    /// <summary>
    /// Comma.
    /// </summary>
    Comma = 44,

    /// <summary>
    /// Minus.
    /// </summary>
    Minus,

    /// <summary>
    /// Period.
    /// </summary>
    Period,

    /// <summary>
    /// Slash.
    /// </summary>
    Slash,

    /// <summary>
    /// Number0.
    /// </summary>
    Number0,

    /// <summary>
    /// Number1.
    /// </summary>
    Number1,

    /// <summary>
    /// Number2.
    /// </summary>
    Number2,

    /// <summary>
    /// Number3.
    /// </summary>
    Number3,

    /// <summary>
    /// Number4.
    /// </summary>
    Number4,

    /// <summary>
    /// Number5.
    /// </summary>
    Number5,

    /// <summary>
    /// Number6.
    /// </summary>
    Number6,

    /// <summary>
    /// Number7.
    /// </summary>
    Number7,

    /// <summary>
    /// Number8.
    /// </summary>
    Number8,

    /// <summary>
    /// Number9.
    /// </summary>
    Number9,

    /// <summary>
    /// Semicolon.
    /// </summary>
    Semicolon = 59,

    /// <summary>
    /// Equal.
    /// </summary>
    Equal = 61,

    /// <summary>
    /// A.
    /// </summary>
    A = 65,

    /// <summary>
    /// B.
    /// </summary>
    B,

    /// <summary>
    /// C.
    /// </summary>
    C,

    /// <summary>
    /// D.
    /// </summary>
    D,

    /// <summary>
    /// E.
    /// </summary>
    E,

    /// <summary>
    /// F.
    /// </summary>
    F,

    /// <summary>
    /// G.
    /// </summary>
    G,

    /// <summary>
    /// H.
    /// </summary>
    H,

    /// <summary>
    /// I.
    /// </summary>
    I,

    /// <summary>
    /// J.
    /// </summary>
    J,

    /// <summary>
    /// K.
    /// </summary>
    K,

    /// <summary>
    /// L.
    /// </summary>
    L,

    /// <summary>
    /// M.
    /// </summary>
    M,

    /// <summary>
    /// N.
    /// </summary>
    N,

    /// <summary>
    /// O.
    /// </summary>
    O,

    /// <summary>
    /// P.
    /// </summary>
    P,

    /// <summary>
    /// Q.
    /// </summary>
    Q,

    /// <summary>
    /// R.
    /// </summary>
    R,

    /// <summary>
    /// S.
    /// </summary>
    S,

    /// <summary>
    /// T.
    /// </summary>
    T,

    /// <summary>
    /// U.
    /// </summary>
    U,

    /// <summary>
    /// V.
    /// </summary>
    V,

    /// <summary>
    /// W.
    /// </summary>
    W,

    /// <summary>
    /// X.
    /// </summary>
    X,

    /// <summary>
    /// Y.
    /// </summary>
    Y,

    /// <summary>
    /// Z.
    /// </summary>
    Z,

    /// <summary>
    /// Left. Bracket
    /// </summary>
    LeftBracket,

    /// <summary>
    /// Back. Slash
    /// </summary>
    BackSlash,

    /// <summary>
    /// Right. Bracket
    /// </summary>
    RightBracket,

    /// <summary>
    /// Grave. Accent
    /// </summary>
    GraveAccent = 96,

    /// <summary>
    /// World. 1
    /// </summary>
    World1 = 161,

    /// <summary>
    /// World. 2
    /// </summary>
    World2,

    /// <summary>
    /// Escape.
    /// </summary>
    Escape = 256,

    /// <summary>
    /// Enter.
    /// </summary>
    Enter,

    /// <summary>
    /// Tab.
    /// </summary>
    Tab,

    /// <summary>
    /// Backspace.
    /// </summary>
    Backspace = 259,

    /// <summary>
    /// Insert.
    /// </summary>
    Insert,

    /// <summary>
    /// Delete.
    /// </summary>
    Delete,

    /// <summary>
    /// Right.
    /// </summary>
    Right,

    /// <summary>
    /// Left.
    /// </summary>
    Left,

    /// <summary>
    /// Down.
    /// </summary>
    Down,

    /// <summary>
    /// Up.
    /// </summary>
    Up,

    /// <summary>
    /// Page. Up
    /// </summary>
    PageUp,

    /// <summary>
    /// Page. Down
    /// </summary>
    PageDown,

    /// <summary>
    /// Home.
    /// </summary>
    Home,

    /// <summary>
    /// End.
    /// </summary>
    End,

    /// <summary>
    /// CapsLock.
    /// </summary>
    CapsLock = 280,

    /// <summary>
    /// Scroll. Lock
    /// </summary>
    ScrollLock,

    /// <summary>
    /// Number. Lock
    /// </summary>
    NumLock,

    /// <summary>
    /// Print. Screen
    /// </summary>
    PrintScreen,

    /// <summary>
    /// Pause.
    /// </summary>
    Pause,

    /// <summary>
    /// F1.
    /// </summary>
    F1 = 290,

    /// <summary>
    /// F2.
    /// </summary>
    F2,

    /// <summary>
    /// F3.
    /// </summary>
    F3,

    /// <summary>
    /// F4.
    /// </summary>
    F4,

    /// <summary>
    /// F5.
    /// </summary>
    F5,

    /// <summary>
    /// F6.
    /// </summary>
    F6,

    /// <summary>
    /// F7.
    /// </summary>
    F7,

    /// <summary>
    /// F8.
    /// </summary>
    F8,

    /// <summary>
    /// F9.
    /// </summary>
    F9,

    /// <summary>
    /// F10.
    /// </summary>
    F10,

    /// <summary>
    /// F11.
    /// </summary>
    F11,

    /// <summary>
    /// F12.
    /// </summary>
    F12,

    /// <summary>
    /// F13.
    /// </summary>
    F13,

    /// <summary>
    /// F14.
    /// </summary>
    F14,

    /// <summary>
    /// F15.
    /// </summary>
    F15,

    /// <summary>
    /// F16.
    /// </summary>
    F16,

    /// <summary>
    /// F17.
    /// </summary>
    F17,

    /// <summary>
    /// F18.
    /// </summary>
    F18,

    /// <summary>
    /// F19.
    /// </summary>
    F19,

    /// <summary>
    /// F20.
    /// </summary>
    F20,

    /// <summary>
    /// F21.
    /// </summary>
    F21,

    /// <summary>
    /// F22.
    /// </summary>
    F22,

    /// <summary>
    /// F23.
    /// </summary>
    F23,

    /// <summary>
    /// F24.
    /// </summary>
    F24,

    /// <summary>
    /// F25.
    /// </summary>
    F25,

    /// <summary>
    /// Keypad. 0
    /// </summary>
    Keypad0 = 320,

    /// <summary>
    /// Keypad. 1
    /// </summary>
    Keypad1,

    /// <summary>
    /// Keypad. 2
    /// </summary>
    Keypad2,

    /// <summary>
    /// Keypad. 3
    /// </summary>
    Keypad3,

    /// <summary>
    /// Keypad. 4
    /// </summary>
    Keypad4,

    /// <summary>
    /// Keypad. 5
    /// </summary>
    Keypad5,

    /// <summary>
    /// Keypad. 6
    /// </summary>
    Keypad6,

    /// <summary>
    /// Keypad. 7
    /// </summary>
    Keypad7,

    /// <summary>
    /// Keypad. 8
    /// </summary>
    Keypad8,

    /// <summary>
    /// Keypad. 9
    /// </summary>
    Keypad9,

    /// <summary>
    /// Keypad. Decimal
    /// </summary>
    KeypadDecimal,

    /// <summary>
    /// Keypad. Divide
    /// </summary>
    KeypadDivide,

    /// <summary>
    /// Keypad. Multiply
    /// </summary>
    KeypadMultiply,

    /// <summary>
    /// Keypad. Subtract
    /// </summary>
    KeypadSubtract,

    /// <summary>
    /// Keypad. Add
    /// </summary>
    KeypadAdd,

    /// <summary>
    /// Keypad. Enter
    /// </summary>
    KeypadEnter,

    /// <summary>
    /// Keypad. Equal
    /// </summary>
    KeypadEqual,

    /// <summary>
    /// Left. Shift
    /// </summary>
    ShiftLeft = 340,

    /// <summary>
    /// Left. Control
    /// </summary>
    ControlLeft,

    /// <summary>
    /// Left. Alt
    /// </summary>
    AltLeft,

    /// <summary>
    /// Left. Super
    /// </summary>
    SuperLeft,

    /// <summary>
    /// Right. Shift
    /// </summary>
    ShiftRight,

    /// <summary>
    /// Right. Control
    /// </summary>
    ControlRight,

    /// <summary>
    /// Right. Alt
    /// </summary>
    AltRight,

    /// <summary>
    /// Right. Super
    /// </summary>
    SuperRight,

    /// <summary>
    /// Menu.
    /// </summary>
    Menu,
}
