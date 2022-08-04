// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Windowing;

public interface IWindow
{
    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the window icon.
    /// </summary>
    ReadOnlyMemory<byte> Icon { get; set; }

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    Size Size { get; set; }

    /// <summary>
    /// Gets or sets the minimum size of this window.
    /// </summary>
    Size MinimumSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of this window.
    /// </summary>
    Size MaximumSize { get; set; }

    /// <summary>
    /// Gets or sets whether this window can be resized.
    /// </summary>
    bool Resizable { get; set; }

    /// <summary>
    /// Gets whether this window is currently focused.
    /// </summary>
    bool Focused { get; }

    /// <summary>
    /// Gets or sets whether this window is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Called when the window loads.
    /// </summary>
    event Action OnLoad;

    /// <summary>
    /// Called when the window closes.
    /// </summary>
    event Action OnClose;

    /// <summary>
    /// Called when the window requests to be closed. Return true to continue closing.
    /// </summary>
    event Func<bool> OnCloseRequested;

    /// <summary>
    /// Called when the window resizes.
    /// </summary>
    event Action<Size> OnResize;

    /// <summary>
    /// Called when the window focus changes.
    /// </summary>
    event Action<bool> OnFocusChanged;

    /// <summary>
    /// Called when a file has been dropped to this window.
    /// </summary>
    event Action<string[]> OnDataDropped;

    /// <summary>
    /// Process events for this window.
    /// </summary>
    void DoEvents();
}
