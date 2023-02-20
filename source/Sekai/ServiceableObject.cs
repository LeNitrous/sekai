// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

#pragma warning disable 0618

using System;
using System.ComponentModel;
using Sekai.Allocation;

namespace Sekai;

/// <summary>
/// The base class for all serviceable objects which can use the <see cref="ResolvedAttribute"/> to resolve services from the <see cref="Game"/> service container.
/// </summary>
public abstract class ServiceableObject : DisposableObject
{
    private readonly ServiceContractResolver resolver;

    protected ServiceableObject()
    {
        resolver = CreateContractResolver();
        Host.Current.Threads.Game.Send(resolver.Resolve);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Host.Current.Threads.Game.Send(resolver.Dispose);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Overridden by source generators")]
    protected virtual ServiceContractResolver CreateContractResolver() => new DefaultContractResolver(this);

    private class DefaultContractResolver : ServiceContractResolver
    {
        public DefaultContractResolver(ServiceableObject target)
            : base(target, null!)
        {
        }

        public override void Resolve()
        {
            // Intentionally no-ops to prevent calling the base implementation.
        }

        protected override void Dispose(bool disposing)
        {
            // Intentionally no-ops to prevent calling the base implementation.
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Used by source generators")]
    protected abstract class ServiceContractResolver : DisposableObject
    {
        protected readonly ServiceableObject Target;

        private readonly ServiceContractResolver resolver;

        protected ServiceContractResolver(ServiceableObject target, ServiceContractResolver resolver)
        {
            Target = target;
            this.resolver = resolver;
        }

        public virtual void Resolve()
        {
            resolver.Resolve();
        }

        protected override void Dispose(bool disposing)
        {
            resolver.Dispose();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Used by source generators")]
    protected abstract class ServiceContractResolver<T> : ServiceContractResolver
        where T : ServiceableObject
    {
        public new T Target => (T)base.Target;

        protected ServiceContractResolver(T target, ServiceContractResolver resolver)
            : base(target, resolver)
        {
        }
    }
}
