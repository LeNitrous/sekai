// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Threading;

namespace Sekai.Engine.Platform;

public partial class Host<T>
{
    private class MainUpdateThread : UpdateThread
    {
        private readonly GameSystemCollection systems;

        public MainUpdateThread(GameSystemCollection systems)
            : base("Main")
        {
            this.systems = systems;
        }

        protected override void OnUpdateFrame(double delta)
        {
            foreach (var system in systems)
            {
                if (!system.Activated)
                    continue;

                if (system is IUpdateable updateable)
                    updateable.Update(delta);
            }
        }
    }

    private class MainRenderThread : RenderThread
    {
        private readonly GameSystemCollection systems;

        public MainRenderThread(GameSystemCollection systems)
            : base("Main")
        {
            this.systems = systems;
        }

        protected override void OnRenderFrame()
        {
            foreach (var system in systems)
            {
                if (!system.Activated)
                    continue;

                if (system is IRenderable renderable)
                    renderable.Render();
            }
        }
    }
}
