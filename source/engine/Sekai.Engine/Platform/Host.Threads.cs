// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Threading;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Platform;

public partial class Host<T>
{
    private class MainUpdateThread : UpdateThread
    {
        private readonly SystemCollection<GameSystem> systems;

        public MainUpdateThread(SystemCollection<GameSystem> systems)
            : base("Main")
        {
            this.systems = systems;
        }

        protected override void OnUpdateFrame(double delta)
        {
            foreach (var system in systems)
            {
                if (!system.IsAlive)
                    continue;

                if (system is IUpdateable updateable)
                    updateable.Update(delta);
            }
        }
    }

    private class MainRenderThread : RenderThread
    {
        private readonly SystemCollection<GameSystem> systems;
        private readonly IGraphicsDevice context;
        private readonly ICommandQueue queue;

        public MainRenderThread(SystemCollection<GameSystem> systems, IGraphicsDevice context)
            : base("Main")
        {
            queue = context.Factory.CreateCommandQueue();
            this.systems = systems;
            this.context = context;
        }

        protected override void OnRenderFrame()
        {
            try
            {
                queue.Begin();
                queue.SetFramebuffer(context.SwapChain.Framebuffer);
                queue.Clear(0, new ClearInfo(new Color4(0, 0, 0, 1f)));
                queue.End();
                context.Submit(queue);

                foreach (var system in systems)
                {
                    if (!system.IsAlive)
                        continue;

                    if (system is IRenderable renderable)
                        renderable.Render();
                }
            }
            finally
            {
                context.SwapBuffers();
            }
        }

        protected override void Destroy()
        {
            queue.Dispose();
        }
    }
}
