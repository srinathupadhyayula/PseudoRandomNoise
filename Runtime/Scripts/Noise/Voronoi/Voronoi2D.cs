using AbstractInterfaces;
using Hash;
using Types;
using Unity.Mathematics;
using Utilities;

namespace Noise.Voronoi
{
    public struct Voronoi2D<TLattice, TVoronoiDistance, TVoronoiFunction> : INoise
        where TLattice : struct, ILattice
        where TVoronoiDistance : struct, IVoronoiDistance
        where TVoronoiFunction : struct, IVoronoiFunction
    {

        public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
        {
            var l = default(TLattice);
            var d = default(TVoronoiDistance);
            LatticeSpan4
                x = l.GetLatticeSpan4(positions.c0, frequency), z = l.GetLatticeSpan4(positions.c2, frequency);

            float4x2 minima = 2f;
            for (int u = -1; u <= 1; u++)
            {
                SmallXxHash4 hx      = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
                float4       xOffset = u - x.g0;
                for (int v = -1; v <= 1; v++)
                {
                    SmallXxHash4 h       = hx.Eat(l.ValidateSingleStep(z.p0 + v, frequency));
                    float4       zOffset = v - z.g0;
                    minima = NoiseUtils.UpdateVoronoiMinima(minima, d.GetDistance(h.Floats01A + xOffset, h.Floats01B + zOffset));
                    minima = NoiseUtils.UpdateVoronoiMinima(minima, d.GetDistance(h.Floats01C + xOffset, h.Floats01D + zOffset));
                }
            }

            return default(TVoronoiFunction).Evaluate(d.Finalize2D(minima));
        }
    }
}