// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using System.Text;
using Pastel;

namespace Sekai.Logging;

/// <summary>
/// A <see cref="LogSink"/> that writes to the <see cref="Console"/> standard output.
/// </summary>
public sealed partial class LogSinkConsole : LogSink
{
    public LogSinkConsole()
    {
        Format = new LogFormatterConsole();
        Console.OutputEncoding = Encoding.UTF8;

        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            AllocConsole();
    }

    public override void Flush()
    {
        Console.Out.Flush();
    }

    public override void Clear()
    {
        Console.Clear();
    }

    protected override void Write(string message)
    {
        Console.WriteLine(message);
    }

    protected override void Dispose(bool disposing)
    {
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            FreeConsole();
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    private class LogFormatterConsole : LogFormatterDefault
    {
        public override string Format(LogMessage message)
        {
            return base.Format(message).Pastel(ConsoleColor.DarkGray);
        }

        protected override string FormatMessage(LogMessage m)
        {
            ConsoleColor color;

            switch (m.Level)
            {
                default:
                    color = ConsoleColor.White;
                    break;

                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;

                case LogLevel.Debug:
                    color = ConsoleColor.Gray;
                    break;

                case LogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
            }

            return base.FormatMessage(m).Pastel(color);
        }

        protected override string FormatException(LogMessage m)
        {
            return base.FormatException(m).Pastel(ConsoleColor.Red);
        }

        protected override string FormatLogLevel(LogMessage m)
        {
            switch (m.Level)
            {
                default:
                case LogLevel.Verbose:
                    return "💬";

                case LogLevel.Debug:
                    return "💭";

                case LogLevel.Information:
                    return "📢";

                case LogLevel.Warning:
                    return "⚠️";

                case LogLevel.Error:
                    return "💥";
            }
        }
    }
}
