// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Pastel;
using Sekai.Mathematics;

namespace Sekai.Logging;

internal sealed partial class LogWriterConsole : LogWriter
{
    private bool isDisposed;
    private readonly LogWriterConsoleStream streamOut;
    private readonly LogWriterConsoleStream streamErr;

    public LogWriterConsole()
    {
        if (RuntimeInfo.IsWindows && RuntimeInfo.IsDebug)
        {
            AllocConsole();
        }

        streamErr = new LogWriterConsoleStream(Console.OpenStandardError());
        streamOut = new LogWriterConsoleStream(Console.OpenStandardOutput());
    }

    public override void Flush()
    {
        streamErr.Flush();
        streamOut.Flush();
    }

    public override void Write(LogMessage message)
    {
        LogWriterConsoleStream writer;

        switch (message.Level)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Verbose:
                writer = streamOut;
                break;

            case LogLevel.Warning:
            case LogLevel.Error:
            case LogLevel.Critical:
                writer = streamErr;
                break;

            default:
                throw new NotSupportedException($"Log level {message.Level} is not supported.");
        }

        writer.Write(message);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        if (RuntimeInfo.IsWindows && RuntimeInfo.IsDebug)
        {
            FreeConsole();
        }

        streamOut.Dispose();
        streamErr.Dispose();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    private class LogWriterConsoleStream : LogWriterText
    {
        public LogWriterConsoleStream(Stream stream)
            : base(stream)
        {
        }

        protected override string Format(LogMessage m) => base.Format(m).Pastel(Color.DarkSlateGray);
        protected override string? GetLevel(LogMessage m) => icons[m.Level];
        protected override string? GetContent(LogMessage m) => base.GetContent(m)?.Pastel(foregroundColors[m.Level]);
        protected override string? GetTimestamp(LogMessage m) => base.GetTimestamp(m).Pastel(Color.White);
        protected override string? GetException(LogMessage m) => base.GetException(m)?.Pastel(foregroundColors[m.Level]);

        private static readonly Dictionary<LogLevel, string> icons = new()
        {
            { LogLevel.Trace,       "📻" },
            { LogLevel.Debug,       "⚙️" },
            { LogLevel.Verbose,     "💬" },
            { LogLevel.Warning,     "⚠️" },
            { LogLevel.Error,       "💢" },
            { LogLevel.Critical,    "💥" },
        };

        private static readonly Dictionary<LogLevel, string> foregroundColors = new()
        {
            { LogLevel.Trace,       "#292929" },
            { LogLevel.Debug,       "#545454" },
            { LogLevel.Verbose,     "#c2c2c2" },
            { LogLevel.Warning,     "#ebd513" },
            { LogLevel.Error,       "#db4242" },
            { LogLevel.Critical,    "#ff0000" },
        };
    }
}
