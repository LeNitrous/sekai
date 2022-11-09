// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Pastel;

namespace Sekai.Logging;

public class LogListenerConsole : LogListenerTextWriter
{
    protected override Func<Exception, string> FormatException => formatException;
    protected override Func<DateTime, string> FormatTimestamp => formatTimestamp;
    protected override Func<LogLevel, string> FormatLogLevel => formatLogLevel;
    protected override Func<string, string> FormatMessage => formatMessage;

    public LogListenerConsole()
        : base(Console.Out)
    {
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            AllocConsole();
    }

    protected override void Destroy()
    {
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            FreeConsole();
    }

    protected override string GetTextFormatted(LogMessage message) => base.GetTextFormatted(message).Pastel(gray1);

    private string formatTimestamp(DateTime time) => base.FormatTimestamp(time).Pastel(gray3);
    private string formatLogLevel(LogLevel level) => "⬤".Pastel(getColorForLevel(level));
    private string formatMessage(string message) => base.FormatMessage(message).Pastel(white);
    private string formatException(Exception exception) => string.Join('\n', exception.ToString().Split('\n').Select(line => line.Pastel(critical)));

    private static readonly string critical = @"#eb4034";
    private static readonly string verbose = @"#3794ff";
    private static readonly string warning = @"#f1ff54";
    private static readonly string gray1 = @"#363636";
    private static readonly string gray2 = @"#6b6b6b";
    private static readonly string gray3 = @"#828282";
    private static readonly string white = @"#ffffff";

    private static string getColorForLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => gray2,
            LogLevel.Verbose => verbose,
            LogLevel.Information => gray3,
            LogLevel.Warning => warning,
            LogLevel.Error => critical,
            _ => gray3,
        };
    }

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeConsole();
}
