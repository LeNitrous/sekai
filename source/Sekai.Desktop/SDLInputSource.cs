// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input;
using static SDL2.SDL;

namespace Sekai.Desktop;

internal sealed class SDLInputSource : InputSource
{
    public SDLInputSource(SDLWindow window)
    {
        window.SDLEvent += handleSDLEvent;
    }

    private void handleSDLEvent(SDL_Event e)
    {
        switch (e.type)
        {
            case SDL_EventType.SDL_KEYUP:
                Enqueue(new KeyboardEvent(e.key.keysym.ToKey(), false));
                break;

            case SDL_EventType.SDL_KEYDOWN:
                Enqueue(new KeyboardEvent(e.key.keysym.ToKey(), true));
                break;

            case SDL_EventType.SDL_MOUSEMOTION:
                Enqueue(new MouseMotionEvent(new(e.motion.x, e.motion.y)));
                break;

            case SDL_EventType.SDL_MOUSEBUTTONUP:
                Enqueue(new MouseButtonEvent(e.button.ToButton(), true));
                break;

            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                Enqueue(new MouseButtonEvent(e.button.ToButton(), false));
                break;
        }
    }
}
