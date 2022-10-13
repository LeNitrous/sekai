using Sekai.Framework.Threading;

namespace Sekai.Engine.Threading;

public abstract class GameThread : FrameworkThread
{
    protected readonly Game Game;

    public GameThread(string name)
        : base(name)
    {
        Game = Game.Current;
    }
}
