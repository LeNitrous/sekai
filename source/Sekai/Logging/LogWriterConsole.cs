// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Pastel;
using Sekai.Platform;

namespace Sekai.Logging;

internal sealed partial class LogWriterConsole : LogWriter, IDisposable
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

    protected override void Flush()
    {
        streamErr.Flush();
        streamOut.Flush();
    }

    protected override void Write(LogMessage message)
    {
        LogWriterConsoleStream writer;

        switch (message.Level)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Information:
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

    ~LogWriterConsole()
    {
        Dispose();
    }

    public void Dispose()
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

    private class LogWriterConsoleStream : LogWriterStream
    {
        public LogWriterConsoleStream(Stream stream)
            : base(stream)
        {
        }

        public new void Write(LogMessage message)
        {
            base.Write(message);
        }

        public new void Flush()
        {
            base.Flush();
        }

        protected override string Format(LogMessage message)
        {
            var builder = new StringBuilder();
            builder.Append($"[{iconMapping[message.Level]}] [{message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss tt").Pastel(Color.White)}]".Pastel(Color.SlateGray));

            if (message.Message is not null)
            {
                builder.Append(' ');
                builder.Append(message.Message.ToString().Pastel(foregroundColorMapping[message.Level]));
            }

            if (message.Exception is not null)
            {
                if (message.Message is not null)
                {
                    builder.AppendLine();
                }
                else
                {
                    builder.Append(" An exception has occured.".Pastel(foregroundColorMapping[message.Level]));
                }

                builder.Append(message.Exception.ToString().Pastel(foregroundColorMapping[message.Level]));
            }

            builder.AppendLine();

            return builder.ToString();
        }

        private static readonly Dictionary<LogLevel, string> iconMapping = new()
        {
            { LogLevel.Trace,       "📻" },
            { LogLevel.Debug,       "⚙️" },
            { LogLevel.Information, "💬" },
            { LogLevel.Warning,     "⚠️" },
            { LogLevel.Error,       "💢" },
            { LogLevel.Critical,    "💥" },
        };

        private static readonly Dictionary<LogLevel, string> foregroundColorMapping = new()
        {
            { LogLevel.Trace,       "#292929" },
            { LogLevel.Debug,       "#545454" },
            { LogLevel.Information, "#c2c2c2" },
            { LogLevel.Warning,     "#ebd513" },
            { LogLevel.Error,       "#db4242" },
            { LogLevel.Critical,    "#ff0000" },
        };
    }
}
