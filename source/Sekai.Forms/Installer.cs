// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sekai.Forms;

internal static class Installer
{
    public static void Install()
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = $"{AppDomain.CurrentDomain.BaseDirectory}/redist/{RuntimeInformation.RuntimeIdentifier}/WindowsAppRuntimeInstall.exe",
            CreateNoWindow = true,
            UseShellExecute = true,
            Arguments = "-q",
            WindowStyle = ProcessWindowStyle.Hidden,
        });

        if (process is null)
            throw new InstallerException(@"Failed to start installer.");

        process.WaitForExit(TimeSpan.FromSeconds(30));

        if (process.ExitCode != 0)
            throw new InstallerException($"Failed to install runtime. Installer exited with code: {process.ExitCode:0x}");
    }
}

internal class InstallerException : Exception
{
    public InstallerException(string? message)
        : base(message)
    {
    }
}
