// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sekai.Graphics;

public sealed class ShaderReflection
{
    [JsonPropertyName("entryPoints")]
    public Entry[] EntryPoints { get; } = Array.Empty<Entry>();

    [JsonPropertyName("types")]
    public Dictionary<string, Type> Types { get; } = new();

    [JsonPropertyName("inputs")]
    public Attribute[] Inputs { get; } = Array.Empty<Attribute>();

    [JsonPropertyName("outputs")]
    public Attribute[] Outputs { get; } = Array.Empty<Attribute>();

    [JsonPropertyName("ubos")]
    public Resource[] Buffers { get; } = Array.Empty<Resource>();

    [JsonPropertyName("images")]
    public Resource[] Images { get; } = Array.Empty<Resource>();

    [JsonPropertyName("separate_images")]
    public Resource[] Textures { get; } = Array.Empty<Resource>();

    [JsonPropertyName("separate_samplers")]
    public Resource[] Samplers { get; } = Array.Empty<Resource>();

    [JsonConstructor]
    public ShaderReflection(Entry[] entryPoints, Dictionary<string, Type> types, Attribute[] inputs, Attribute[] outputs, Resource[] buffers, Resource[] images, Resource[] textures, Resource[] samplers)
    {
        Types = types;
        Inputs = inputs;
        Images = images;
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
        public Member[] Members { get; } = Array.Empty<Member>();

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
