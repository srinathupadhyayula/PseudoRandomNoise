using AbstractInterfaces;
using Unity.Mathematics;

namespace Noise.Voronoi.DistanceFunctions
{
    public struct ChebyshevDistance : IVoronoiDistance
    {
        public float4   GetDistance(float4  x)                     => math.abs(x);
        public float4   GetDistance(float4  x, float4 y)           => math.max(math.abs(x),                        math.abs(y));
        public float4   GetDistance(float4  x, float4 y, float4 z) => math.max(math.max(math.abs(x), math.abs(y)), math.abs(z));
        public float4x2 Finalize1D(float4x2 minima) => minima;
        public float4x2 Finalize2D(float4x2 minima) => minima;
        public float4x2 Finalize3D(float4x2 minima) => minima;
    }
}