// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Pastel;

namespace Sekai.Framework.Logging;

public class LogListenerConsole : LogListenerTextWriter
{
    public LogListenerConsole()
        : base(Console.Out)
    {
    }

    protected override Func<Exception, string> FormatException => formatException;
    protected override Func<DateTime, string> FormatTimestamp => formatTimestamp;
    protected override Func<LogLevel, string> FormatLogLevel => formatLogLevel;
    protected override Func<string, string> FormatMessage => formatMessage;

    protected override string GetTextFormatted(LogMessage message)
    {
        return base.GetTextFormatted(message).Pastel(gray1);
    }

    private string formatTimestamp(DateTime time)
    {
        return base.FormatTimestamp(time).Pastel(gray3);
    }

    private string formatLogLevel(LogLevel level)
    {
        return "⬤".Pastel(getColorForLevel(level));
    }

    private string formatMessage(string message)
    {
        return base.FormatMessage(message).Pastel(white);
    }

    private string formatException(Exception exception)
    {
        return base.FormatException(exception).Pastel(critical);
    }

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
}
