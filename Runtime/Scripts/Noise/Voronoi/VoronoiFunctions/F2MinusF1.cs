using AbstractInterfaces;
using Unity.Mathematics;

namespace Noise.Voronoi.VoronoiFunctions
{
    public struct F2MinusF1 : IVoronoiFunction
    {
        public float4 Evaluate(float4x2 distances) => distances.c1 - distances.c0;
    }
}