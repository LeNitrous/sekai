// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Windowing;

public interface IWindow : IView
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
    /// Gets or sets the state of this window.
    /// </summary>
    WindowState State { get; set; }

    /// <summary>
    /// Gets or sets the window borders.
    /// </summary>
    WindowBorder Border { get; set; }

    /// <summary>
    /// Gets or sets whether this window is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Called when the view resizes.
    /// </summary>
    event Action<Size> OnResize;

    /// <summary>
    /// Called when a file has been dropped to this window.
    /// </summary>
    event Action<string[]> OnDataDropped;
}
