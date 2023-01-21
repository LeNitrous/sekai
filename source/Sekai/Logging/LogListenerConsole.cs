// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Pastel;

namespace Sekai.Logging;

public partial class LogListenerConsole : LogListenerTextWriter
{
    protected override Func<Exception, string> FormatException => formatException;
    protected override Func<DateTime, string> FormatTimestamp => formatTimestamp;
    protected override Func<LogLevel, string> FormatLogLevel => formatLogLevel;
    protected override Func<object?, string> FormatMessage => formatMessage;

    public LogListenerConsole()
        : base(Console.Out)
    {
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
        {
            AllocConsole();
            SetConsoleOutputCP(65001);
        }
    }

    protected override void Destroy()
    {
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            FreeConsole();
    }

    protected override string GetTextFormatted(LogMessage message) => base.GetTextFormatted(message).Pastel(gray1);

    private string formatTimestamp(DateTime time) => base.FormatTimestamp(time).Pastel(gray3);
    private string formatLogLevel(LogLevel level) => "⬤".Pastel(getColorForLevel(level));
    private string formatMessage(object? message) => base.FormatMessage(message).Pastel(white);
    private string formatException(Exception exception) => string.Join('\n', exception.ToString().Split('\n').Select(line => line.Pastel(critical)));

    private static readonly string critical = @"#eb4034";
    private static readonly string verbose = @"#3794ff";
    private static readonly string warning = @"#f1ff54";
    private static readonly string gray1 = @"#363636";
    private static readonly string gray2 = @"#6b6b6b";
    private static readonly string gray3 = @"#828282";
    private static readonly string white = @"#ffffff";

    private static string getColorForLevel(LogLevel level) => level switch
    {
        LogLevel.Debug => gray2,
        LogLevel.Verbose => verbose,
        LogLevel.Information => gray3,
        LogLevel.Warning => warning,
        LogLevel.Error => critical,
        _ => gray3,
    };


    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleOutputCP(uint wCodePageID);
}
