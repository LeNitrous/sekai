// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Forms;

public static class FormsGameBuilderExtensions
{
    public static GameBuilder<T> UseForms<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .AddPreBuildAction(Installer.Install)
            .AddLoadAction(() => Bootstrap.Initialize(0x00010000))
            .AddExitAction(() => Bootstrap.Shutdown())
            .UseView<FormsWindow>();
    }
}
