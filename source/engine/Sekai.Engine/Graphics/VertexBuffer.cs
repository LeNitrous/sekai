// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Reflection;
using Sekai.Engine.Annotations;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Graphics;

/// <summary>
/// Determines vertex data and layout.
/// </summary>
public class VertexBuffer : BindableBuffer
{
    internal VertexLayoutDescription Layout { get; private set; }

    public VertexBuffer(IGraphicsDevice device, VertexLayout layout, int count)
        : base(device, layout.Stride * count, BufferUsage.Vertex)
    {
        Layout = layout.Build();
    }

    /// <summary>
    /// Binds this vertex buffer to the command queue at a given index.
    /// </summary>
    public override void Bind(ICommandQueue queue)
    {
        queue.SetVertexBuffer(0, Buffer);
    }
}

public class VertexBuffer<T> : VertexBuffer
    where T : unmanaged, IVertex
{
    private static readonly VertexLayout layout = createLayout(typeof(T));

    public VertexBuffer(IGraphicsDevice device, int count)
        : base(device, layout, count)
    {
    }

    /// <summary>
    /// Sets the data for this vertex buffer.
    /// </summary>
    public void SetData(T data, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, ref data, (uint)offset);
    }

    /// <summary>
    /// Sets the data for this vertex buffer.
    /// </summary>
    public void SetData(ref T data, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, ref data, (uint)offset);
    }

    /// <summary>
    /// Sets the data for this vertex buffer.
    /// </summary>
    public void SetData(T[] data, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, data, (uint)offset);
    }

    private static VertexLayout createLayout(Type type)
    {
        var layout = new VertexLayout();

        foreach (var member in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var attrib = member.GetCustomAttribute<VertexMemberAttribute>();

            if (attrib is null)
                continue;

            string name = attrib.Name ?? member.Name;

            if (member.FieldType.IsAssignableTo(typeof(IVertex)))
            {
                layout.Add(createLayout(member.FieldType));
            }
            else
            {
                if (attrib.Format.HasValue)
                {
                    layout.Add(name, attrib.Format.Value);
                }
                else
                {
                    layout.Add(member.FieldType, name);
                }
            }
        }

        return layout;
    }
}
