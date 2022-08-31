// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Effects;

public class Effect : FrameworkObject
{
    /// <summary>
    /// The effect name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The effect type.
    /// </summary>
    public EffectType Type { get; }

    /// <summary>
    /// The graphics API this effect is compiled against.
    /// </summary>
    public GraphicsAPI API { get; }

    /// <summary>
    /// The declared vertex inputs for this effect.
    /// </summary>
    public IReadOnlyList<EffectMember> Stages { get; }

    /// <summary>
    /// The declared resources for this effect.
    /// </summary>
    public IReadOnlyList<EffectMember> Resources { get; }

    internal IGraphicsDevice Device { get; }
    internal IShader[] Shaders { get; }
    private byte[][] byteCodes { get; }

    internal Effect(IGraphicsDevice device, string name, EffectType type, GraphicsAPI api, IReadOnlyList<EffectMember> stages, IReadOnlyList<EffectMember> resources, byte[] vertCode, byte[] fragCode)
        : this(device, name, type, api, stages, resources, new[] { ShaderStage.Vertex, ShaderStage.Fragment }, new[] { vertCode, fragCode })
    {
    }

    internal Effect(IGraphicsDevice device, string name, EffectType type, GraphicsAPI api, IReadOnlyList<EffectMember> stages, IReadOnlyList<EffectMember> resources, byte[] compCode)
        : this(device, name, type, api, stages, resources, new[] { ShaderStage.Compute }, new[] { compCode })
    {
    }

    private Effect(IGraphicsDevice device, string name, EffectType type, GraphicsAPI api, IReadOnlyList<EffectMember> stages, IReadOnlyList<EffectMember> resources, ShaderStage[] shaderStages, byte[][] codes)
    {
        API = api;
        Name = name;
        Type = type;
        Device = device;
        Stages = stages;
        Shaders = new IShader[codes.Length];
        byteCodes = new byte[2][];
        Resources = resources;

        for (int i = 0; i < codes.Length; i++)
        {
            var descriptor = new ShaderDescription { Code = codes[i], Stage = shaderStages[i], EntryPoint = API != GraphicsAPI.Metal ? "main" : "main0" };
            Shaders[i] = device.Factory.CreateShader(ref descriptor);
            byteCodes[i] = codes[i];
        }
    }

    public byte[] Serialize()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write(MAGIC);
        writer.Write(Name);
        writer.Write((byte)Type);
        writer.Write((byte)(Stages.Count + Resources.Count));
        writer.Write(byteCodes[0]?.Length ?? 0);
        writer.Write(byteCodes[1]?.Length ?? 0);

        foreach (var member in Stages.Concat(Resources))
        {
            writer.Write(member.Name);
            writer.Write(member.Type);
            writer.Write((byte)member.Size);
            writer.Write((byte)member.Flags);
        }

        writer.Write(byteCodes[0]);
        writer.Write(byteCodes[1]);
        return stream.ToArray();
    }

    public static Effect Load(IGraphicsDevice device, byte[] code)
    {
        if (code.Length < 4 ||
            code[0] != MAGIC[0] ||
            code[1] != MAGIC[1] ||
            code[2] != MAGIC[2] ||
            code[3] != MAGIC[3])
        {
            throw new InvalidOperationException(@"Invalid effect byte code.");
        }

        using var stream = new MemoryStream(code);
        using var reader = new BinaryReader(stream);
        reader.BaseStream.Position = 3;

        string name = reader.ReadString();
        var type = (EffectType)reader.ReadByte();
        var api = (GraphicsAPI)reader.ReadByte();

        if (device.GraphicsAPI != api)
            throw new InvalidOperationException(@"Invalid effect.");

        int memberCount = reader.ReadByte();
        int byteCountA = reader.ReadInt32();
        int byteCountB = reader.ReadInt32();

        var stages = new List<EffectMember>();
        var resources = new List<EffectMember>();

        for (int i = 0; i < memberCount; i++)
        {
            string memberName = reader.ReadString();
            string memberType = reader.ReadString();

            byte size = reader.ReadByte();

            var flags = (EffectMemberFlags)reader.ReadByte();
            var member = EffectMember.Create(memberName, memberType, size, flags);

            if (flags.HasFlag(EffectMemberFlags.Staged))
            {
                stages.Add(member);
            }
            else
            {
                resources.Add(member);
            }
        }

        byte[] codeA = reader.ReadBytes(byteCountA);

        if (type == EffectType.Graphics)
        {
            byte[] codeB = reader.ReadBytes(byteCountB);
            return new Effect(device, name, type, api, stages, resources, codeA, codeB);
        }
        else
        {
            return new Effect(device, name, type, api, stages, resources, codeA);
        }
    }

    public static readonly byte[] MAGIC = new byte[] { 83, 75, 83, 72 };
}
