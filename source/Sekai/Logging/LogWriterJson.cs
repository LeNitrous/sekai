// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sekai.Logging;

public sealed class LogWriterJson : LogWriterStream
{
    private const string prop_time = "time";
    private const string prop_level = "severity";
    private const string prop_content = "message";
    private const string prop_exception = "exception";
    private const string prop_exception_type = "type";
    private const string prop_exception_trace = "trace";
    private readonly Utf8JsonWriter writer;
    private readonly List<LogMessage> messages = new();

    public LogWriterJson(Stream stream, bool leaveOpen = false)
        : base(stream, leaveOpen)
    {
        writer = new Utf8JsonWriter(stream, options);
    }

    public override void Write(LogMessage message)
    {
        messages.Add(message);
    }

    public override void Flush()
    {
        writer.WriteStartArray();

        for (int i = 0; i < messages.Count; i++)
        {
            var message = messages[i];

            writer.WriteStartObject();
            writer.WriteString(prop_time, message.Timestamp);
            writer.WriteNumber(prop_level, (int)message.Level);
            writer.WriteString(prop_content, message.Content?.ToString());

            if (message.Exception is not null)
            {
                writer.WritePropertyName(prop_exception);
                writer.WriteStartObject();
                writer.WriteString(prop_exception_type, message.Exception.GetType().Name);
                writer.WriteString(prop_exception_trace, message.Exception.StackTrace);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();
    }

    private static readonly JsonWriterOptions options = new() { Indented = true };
}
