// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;

namespace Sekai.Framework;

/// <summary>
/// A component variant that is capable of updating per-frame.
/// </summary>
public abstract class Behavior : Component
{
    /// <summary>
    /// Called exactly once every frame.
    /// </summary>
    /// <param name="delta">The time in milliseconds between two frames.</param>
    public virtual void Update(double delta)
    {
    }

    /// <summary>
    /// Called once or possibly multiple times every frame.
    /// </summary>
    public virtual void FixedUpdate()
    {
    }

    /// <summary>
    /// Called exactly once every frame after <see cref="Update(double)"/>.
    /// </summary>
    public virtual void LateUpdate(double delta)
    {
    }

    protected override void OnAttach()
    {
        base.OnAttach();
        Game.Resolve<BehaviorService>().Add(this);
    }

    protected override void OnDetach()
    {
        base.OnDetach();
        Game.Resolve<BehaviorService>().Remove(this);
    }
}
