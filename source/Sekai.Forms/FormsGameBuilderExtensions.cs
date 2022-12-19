// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace Sekai.Forms;

public static class FormsGameBuilderExtensions
{
    public static GameBuilder<T> UseForms<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .AddPreBuildAction(() =>
            {
                Installer.Install();
                Bootstrap.Initialize(0x00010000);
            })
            .AddExitAction(Bootstrap.Shutdown)
            .UseView<FormsWindow>();
    }
}
