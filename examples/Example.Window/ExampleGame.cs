// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using System.Numerics;
using Sekai.Engine;
using Sekai.Engine.Annotations;
using Sekai.Engine.Assets;
using Sekai.Engine.Effects;
using Sekai.Engine.Graphics;
using Sekai.Engine.Rendering;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;

namespace Example.Window;

public class ExampleGame : Game
{
    public override void Load()
    {
        var assets = Services.Resolve<AssetLoader>();
        var scenes = Services.Resolve<SceneController>();
        var device = Services.Resolve<IGraphicsDevice>();
        var effect = assets.Load<Effect>("engine/shaders/unlit.sksl");

        var scene = new Scene();

        scene
            .CreateEntity()
            .AddComponent(new Camera { Width = 1280, Height = 720 })
            .AddComponent<Transform>()
            .AddComponent<CameraController>();

        var mesh1 = Cube.Generate(device, Vector3.One, Vector2.One);
        mesh1.Material = scene.RenderContext.CreateMaterial(effect);

        scene
            .CreateEntity()
            .AddComponent(new Transform { Position = new Vector3(0, 0, -5) })
            .AddComponent(new MeshComponent { Mesh = mesh1 });

        scenes.Scene = scene;
    }
}

public class CameraController : Behavior
{
    private IMouse mouse = null!;
    private IKeyboard keyboard = null!;
    private Vector2? mousePressPosition;
    private Transform transform = null!;
    private readonly IInputContext input = Game.Current.Services.Resolve<IInputContext>();

    public override void Start()
    {
        mouse = input.Available.OfType<IMouse>().Single();
        keyboard = input.Available.OfType<IKeyboard>().Single();
        transform = Entity.GetCommponent<Transform>()!;
    }

    public override void Update(double delta)
    {
        const float speed = 100.0f;
        float d = (float)delta;

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
