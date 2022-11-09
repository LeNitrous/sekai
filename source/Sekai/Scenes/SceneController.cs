// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Scenes;

public class SceneController : FrameworkObject
{
    public Scene Current => stack.Count == 0 ? null! : stack.Peek();

    private readonly Stack<Scene> stack = new();

    /// <summary>
    /// Pushes a scene as the current.
    /// </summary>
    public void Push(Scene scene)
    {
        if (stack.Contains(scene))
            throw new InvalidOperationException(@"This scene is already pushed.");

        Scene last = null!;

        if (stack.Count != 0)
        {
            last = stack.Peek();
            last.OnSuspending(scene);
        }

        scene.Controller = this;
        stack.Push(scene);

        scene.OnEntering(last);
    }

    internal void Exit(Scene scene)
    {
        if (stack.Count == 0)
            throw new InvalidOperationException(@"The scene stack is currently empty.");

        if (stack.Peek() != scene)
            throw new InvalidOperationException(@"Cannot exit the scene as this is not the current.");

        var last = stack.Pop();
        last.OnExiting(Current);

        if (stack.TryPeek(out var next))
            next.OnResuming(last);
    }

    internal void MakeCurrent(Scene scene)
    {
        if (stack.Count == 0)
            throw new InvalidOperationException(@"The scene stack is currently empty.");

        Scene current;

        while ((current = stack.Peek()) != scene)
        {
            Exit(current);
        }
    }
}
