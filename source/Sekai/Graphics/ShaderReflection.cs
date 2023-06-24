// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Sekai.Graphics;

public sealed class ShaderReflection
{
    [JsonPropertyName("entryPoints")]
    public IReadOnlyList<Entry> EntryPoints { get; } = Array.Empty<Entry>();

    [JsonPropertyName("types")]
    public IDictionary<string, Type> Types { get; } = ImmutableDictionary<string, Type>.Empty;

    [JsonPropertyName("inputs")]
    public IReadOnlyList<Attribute> Inputs { get; } = Array.Empty<Attribute>();

    [JsonPropertyName("outputs")]
    public IReadOnlyList<Attribute> Outputs { get; } = Array.Empty<Attribute>();

    [JsonPropertyName("ubos")]
    public IReadOnlyList<Resource> Buffers { get; } = Array.Empty<Resource>();

    [JsonPropertyName("images")]
    public IReadOnlyList<Resource> Images { get; } = Array.Empty<Resource>();

    [JsonPropertyName("separate_images")]
    public IReadOnlyList<Resource> Textures { get; } = Array.Empty<Resource>();

    [JsonPropertyName("separate_samplers")]
    public IReadOnlyList<Resource> Samplers { get; } = Array.Empty<Resource>();

    [JsonConstructor]
    public ShaderReflection(Entry[] entryPoints, Dictionary<string, Type> types, Attribute[] inputs, Attribute[] outputs, Resource[] buffers, Resource[] textures, Resource[] samplers)
    {
        Types = types;
        Inputs = inputs;
        Outputs = outputs;
        Buffers = buffers;
        Textures = textures;
        Samplers = samplers;
        EntryPoints = entryPoints;
    }

    public sealed class Type
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("members")]
        public IReadOnlyList<Member> Members { get; } = Array.Empty<Member>();

        [JsonConstructor]
        public Type(string name, Member[] members)
        {
            Name = name;
            Members = members;
        }

        public sealed class Member
        {
            [JsonPropertyName("name")]
            public string Name { get; }

            [JsonPropertyName("type")]
            public string Type { get; }

            [JsonPropertyName("offset")]
            public int Offset { get; }

            [JsonConstructor]
            public Member(string name, string type, int offset)
            {
                Name = name;
                Type = type;
                Offset = offset;
            }
        }
    }

    public sealed class Entry
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("mode")]
        public string Mode { get; }

        [JsonConstructor]
        public Entry(string name, string mode)
        {
            Name = name;
            Mode = mode;
        }
    }

    public sealed class Attribute
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("location")]
        public int Location { get; }

        [JsonConstructor]
        public Attribute(string name, string type, int location)
        {
            Name = name;
            Type = type;
            Location = location;
        }
    }

    public sealed class Resource
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("set")]
        public int Set { get; }

        [JsonPropertyName("binding")]
        public int Binding { get; }

        [JsonPropertyName("block_size")]
        public int Size { get; }

        [JsonConstructor]
        public Resource(string name, string type, int set, int binding, int size)
        {
            Set = set;
            Size = size;
            Name = name;
            Type = type;
            Binding = binding;
        }
    }
}

[JsonSerializable(typeof(ShaderReflection))]
internal sealed partial class ShaderReflectionJsonContext : JsonSerializerContext
{
}
