using AbstractInterfaces;
using Unity.Mathematics;
using Utilities;

namespace Types.Shapes
{
    public struct Sphere : IShape
    {
        public Point4 GetPoint4(int i, float resolution, float invResolution)
        {
            float4x2 uv = NoiseUtils.IndexTo4UV(i, resolution, invResolution);
            Point4   p;
            p.positions.c0 = uv.c0 - 0.5f;
            p.positions.c1 = uv.c1 - 0.5f;
            p.positions.c2 = 0.5f  - math.abs(p.positions.c0) - math.abs(p.positions.c1);
            float4 offset = math.max(-p.positions.c2, 0f);
            p.positions.c0 += math.select(-offset, offset, p.positions.c0 < 0f);
            p.positions.c1 += math.select(-offset, offset, p.positions.c1 < 0f);

            float4 scale = 0.5f
                         * math.rsqrt(p.positions.c0 * p.positions.c0 + p.positions.c1 * p.positions.c1 + p.positions.c2 * p.positions.c2);
            p.positions.c0 *= scale;
            p.positions.c1 *= scale;
            p.positions.c2 *= scale;
            p.normals      =  p.positions;
            return p;
        }
    }
}