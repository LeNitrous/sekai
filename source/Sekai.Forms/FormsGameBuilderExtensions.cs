// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Forms;

public static class FormsGameBuilderExtensions
{
    public static GameBuilder<T> UseForms<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        Installer.Install();
        return builder.UseView<FormsWindow>();
    }
}
