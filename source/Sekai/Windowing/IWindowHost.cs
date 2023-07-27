// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Windowing;

/// <summary>
/// An interface for objects capable of hosting other <see cref="IWindow"/>s.
/// </summary>
public interface IWindowHost
{
    /// <summary>
    /// Creates a new window.
    /// </summary>
    IWindow CreateWindow();
}
