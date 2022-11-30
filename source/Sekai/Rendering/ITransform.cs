using System.Numerics;

namespace Sekai.Rendering;

public interface ITransform
{
    Matrix4x4 LocalMatrix { get; }
    Matrix4x4 WorldMatrix { get; }
}
