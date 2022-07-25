using AbstractInterfaces;
using Unity.Mathematics;
using Utilities;

namespace Types.Shapes
{
    public struct Torus : IShape
    {
        public Point4 GetPoint4(int i, float resolution, float invResolution)
        {
            float4x2 uv = NoiseUtils.IndexTo4UV(i, resolution, invResolution);

            const float r1 = 0.375f;
            const float r2 = 0.125f;
            float4      s  = r1 + r2 * math.cos(2f * math.PI * uv.c1);

            Point4 p;
            p.positions.c0 =  s  * math.sin(2f * math.PI * uv.c0);
            p.positions.c1 =  r2 * math.sin(2f * math.PI * uv.c1);
            p.positions.c2 =  s  * math.cos(2f * math.PI * uv.c0);
            p.normals      =  p.positions;
            p.normals.c0   -= r1 * math.sin(2f * math.PI * uv.c0);
            p.normals.c2   -= r1 * math.cos(2f * math.PI * uv.c0);
            return p;
        }
    }
}