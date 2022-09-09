// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Sekai.Engine;
using Sekai.Engine.Annotations;
using Sekai.Engine.Effects;
using Sekai.Engine.Effects.Compiler;
using Sekai.Engine.Graphics;
using Sekai.Engine.Rendering;
using Sekai.Framework.Annotations;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Example.Window;

public class ExampleGame : Game
{
    [Resolved]
    private IGraphicsDevice device { get; set; } = null!;

    [Resolved]
    private VirtualStorage storage { get; set; } = null!;

    [Resolved]
    private EffectCompiler compiler { get; set; } = null!;

    protected override void Load()
    {
        using var stream = storage.Open("engine/shaders/world.sksl", FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        var effect = compiler.Compile(new EffectSource(reader.ReadToEnd()), EffectType.Graphics);

        var camera = new Camera();
        var sceneController = Systems.Get<SceneController>();

        var samplerDescriptor = new SamplerDescription(SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerFilter.MinPoint_MagPoint_MipPoint, ComparisonKind.Never, 0, int.MaxValue, 0, 0, SamplerBorderColor.OpaqueWhite);
        var texture = Texture.Load(device, new Image<Rgba32>(1, 1, Rgba32.ParseHex("FFFFFF")), false);
        var sampler = device.Factory.CreateSampler(ref samplerDescriptor);

        var material = new Material(effect);
        material["Default"].SetTexture("m_Albedo", texture);
        material["Default"].SetSampler("m_Sampler", sampler);

        var mesh = Cube.Generate(device, Vector3.One, Vector2.One);
        mesh.Material = material;

        sceneController.Scene = new Scene
        {
            Camera = camera,
            Children = new[]
            {
                new Entity
                {
                    Components = new Component[]
                    {
                        camera,
                        new CameraController(),
                        new Transform { Position = new Vector3(0, 0, 5) },
                    }
                },
                new Entity
                {
                    Components = new Component[]
                    {
                        new MeshComponent { Mesh = mesh },
                        new Transform { Scale = new Vector3(3) },
                    }
                },
            },
        };
    }
}

public class CameraController : Behavior
{
    [Resolved]
    private IInputContext input { get; set; } = null!;

    private IMouse mouse = null!;
    private IKeyboard keyboard = null!;
    private Vector2? mousePressPosition;
    private Transform transform = null!;

    public override void Update(double delta)
    {
        const float speed = 100.0f;
        float d = MathF.Round((float)delta, 4);

        transform ??= Entity.GetComponent<Transform>()!;

        keyboard ??= input.Available.OfType<IKeyboard>().Single();

        if (keyboard.IsKeyPressed(Key.W))
            transform.Position -= transform.Forward * speed * d;

        if (keyboard.IsKeyPressed(Key.S))
            transform.Position += transform.Forward * speed * d;

        if (keyboard.IsKeyPressed(Key.A))
            transform.Position -= transform.Right * speed * d;

        if (keyboard.IsKeyPressed(Key.D))
            transform.Position += transform.Right * speed * d;

        if (keyboard.IsKeyPressed(Key.Q))
            transform.Position += transform.Up * speed * d;

        if (keyboard.IsKeyPressed(Key.E))
            transform.Position -= transform.Up * speed * d;

        mouse ??= input.Available.OfType<IMouse>().Single();

        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            if (!mousePressPosition.HasValue)
                mousePressPosition = mouse.Position;

            var mouseDelta = mousePressPosition.Value - mouse.Position;
            transform.RotationEuler += new Vector3(mouseDelta, 0) * (speed / 4.0f) * d;

            mousePressPosition = mouse.Position;
        }
        else
        {
            mousePressPosition = null;
        }
    }
}

public struct TextureVertex3D : IVertex
{
    [VertexMember]
    public Vector3 Position;

    [VertexMember]
    public Vector2 TexturePosition;

    public TextureVertex3D(Vector3 position, Vector2 texturePosition)
    {
        Position = position;
        TexturePosition = texturePosition;
    }
}

public static class Cube
{
    private const int face_count = 6;

    private static readonly Vector3[] normals = new[]
    {
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0)
    };

    private static readonly Vector2[] texturePositions = new[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1),
    };

    public static Mesh Generate(IGraphicsDevice device, Vector3 size, Vector2 uvScale)
    {
        var vertices = new TextureVertex3D[face_count * 4];
        int[] indices = new int[face_count * 6];

        var texCoords = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            texCoords[i] = texturePositions[i] * uvScale;
        }

        size /= 2.0f;

        int indexCount = 0;
        int vertexCount = 0;

        for (int i = 0; i < face_count; i++)
        {
            var normal = normals[i];
            var basis = i >= 4 ? Vector3.UnitZ : Vector3.UnitY;
            var side1 = Vector3.Cross(normal, basis);
            var side2 = Vector3.Cross(normal, side1);

            int vbase = i * 4;
            indices[indexCount++] = vbase + 0;
            indices[indexCount++] = vbase + 1;
            indices[indexCount++] = vbase + 2;

            indices[indexCount++] = vbase + 0;
            indices[indexCount++] = vbase + 2;
            indices[indexCount++] = vbase + 3;

            vertices[vertexCount++] = new TextureVertex3D((normal - side1 - side2) * size, texCoords[0]);
            vertices[vertexCount++] = new TextureVertex3D((normal - side1 + side2) * size, texCoords[1]);
            vertices[vertexCount++] = new TextureVertex3D((normal + side1 + side2) * size, texCoords[2]);
            vertices[vertexCount++] = new TextureVertex3D((normal + side1 - side2) * size, texCoords[3]);
        }

        var vertexBuffer = new VertexBuffer<TextureVertex3D>(device, vertices.Length);
        vertexBuffer.SetData(vertices);

        var indexBuffer = new IndexBuffer<int>(device, indices.Length);
        indexBuffer.SetData(indices);

        return new Mesh
        {
            IndexBuffer = indexBuffer,
            VertexBuffer = vertexBuffer,
        };
    }
}
