using AbstractInterfaces;
using Unity.Mathematics;
using Utilities;

namespace Types.Shapes
{
    public struct Plane : IShape
    {
        public Point4 GetPoint4(int i, float resolution, float invResolution)
        {
            float4x2 uv = NoiseUtils.IndexTo4UV(i, resolution, invResolution);
            return new Point4 {positions = math.float4x3(uv.c0 - 0.5f, 0f, uv.c1 - 0.5f), normals = math.float4x3(0f, 1f, 0f)};
        }
    }
}