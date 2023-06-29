// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sekai.Graphics;

public sealed class ShaderReflection
{
    [JsonPropertyName("entryPoints")]
    public Entry[] EntryPoints { get; }

    [JsonPropertyName("types")]
    public Dictionary<string, Type> Types { get; }

    [JsonPropertyName("inputs")]
    public Attribute[] Inputs { get; }

    [JsonPropertyName("outputs")]
    public Attribute[] Outputs { get; }

    [JsonPropertyName("ubos")]
    public Resource[] Uniforms { get; }

    [JsonPropertyName("ssbos")]
    public Resource[] Storages { get; }

    [JsonPropertyName("images")]
    public Resource[] Images { get; }

    [JsonPropertyName("textures")]
    public Resource[] Textures { get; }

    [JsonConstructor]
    public ShaderReflection(Entry[]? entryPoints, Dictionary<string, Type>? types, Attribute[]? inputs, Attribute[]? outputs, Resource[]? uniforms, Resource[]? storages, Resource[]? images, Resource[]? textures)
    {
        Types = types ?? new();
        Inputs = inputs ?? Array.Empty<Attribute>();
        Images = images ?? Array.Empty<Resource>();
        Outputs = outputs ?? Array.Empty<Attribute>();
        Uniforms = uniforms ?? Array.Empty<Resource>();
        Storages = storages ?? Array.Empty<Resource>();
        Textures = textures ?? Array.Empty<Resource>();
        EntryPoints = entryPoints ?? Array.Empty<Entry>();
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

        [JsonPropertyName("readonly")]
        public bool ReadOnly { get; }

        [JsonConstructor]
        public Resource(string name, string type, int set, int binding, int size, bool readOnly)
        {
            Set = set;
            Size = size;
            Name = name;
            Type = type;
            Binding = binding;
            ReadOnly = readOnly;
        }
    }
}

[JsonSerializable(typeof(ShaderReflection))]
internal sealed partial class ShaderReflectionJsonContext : JsonSerializerContext
{
}
