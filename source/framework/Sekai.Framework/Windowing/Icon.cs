using System;

namespace Sekai.Framework.Windowing;

public readonly record struct Icon(int Width, int Height, Memory<byte> Data);
