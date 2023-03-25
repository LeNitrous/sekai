// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sekai.Graphics;

public partial class Effect
{
    private class Reflection
    {
        [JsonPropertyName("entryPoints")]
        public EntryPoint[] EntryPoints { get; } = Array.Empty<EntryPoint>();

        [JsonPropertyName("types")]
        public Dictionary<string, Type> Types { get; }

        [JsonPropertyName("inputs")]
        public Attribute[] Inputs { get; } = Array.Empty<Attribute>();

        [JsonPropertyName("outputs")]
        public Attribute[] Outputs { get; } = Array.Empty<Attribute>();

        [JsonPropertyName("ubos")]
        public Resource[] Buffers { get; } = Array.Empty<Resource>();

        [JsonPropertyName("separate_images")]
        public Resource[] Textures { get; } = Array.Empty<Resource>();

        [JsonPropertyName("separate_samplers")]
        public Resource[] Samplers { get; } = Array.Empty<Resource>();

        [JsonConstructor]
        public Reflection(EntryPoint[] entryPoints, Dictionary<string, Type> types, Attribute[] inputs, Attribute[] outputs, Resource[] buffers, Resource[] textures, Resource[] samplers)
        {
            Types = types;
            Inputs = inputs;
            Outputs = outputs;
            Buffers = buffers;
            Textures = textures;
            Samplers = samplers;
            EntryPoints = entryPoints;
        }

        public class Type
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

            public class Member
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

        public class EntryPoint
        {
            [JsonPropertyName("name")]
            public string Name { get; }

            [JsonPropertyName("mode")]
            public string Mode { get; }

            [JsonConstructor]
            public EntryPoint(string name, string mode)
            {
                Name = name;
                Mode = mode;
            }
        }

        public class Attribute
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

        public class Resource
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
}
