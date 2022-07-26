﻿using AbstractInterfaces;
using Hash;
using Types;
using Unity.Mathematics;
using Utilities;

namespace Noise.Voronoi
{
    public struct Voronoi3D<TLattice, TVoronoiDistance, TVoronoiFunction> : INoise
        where TLattice : struct, ILattice
        where TVoronoiDistance : struct, IVoronoiDistance
        where TVoronoiFunction : struct, IVoronoiFunction
    {

        public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
        {
            var l = default(TLattice);
            var d = default(TVoronoiDistance);
            LatticeSpan4
                x = l.GetLatticeSpan4(positions.c0, frequency)
              , y = l.GetLatticeSpan4(positions.c1, frequency)
              , z = l.GetLatticeSpan4(positions.c2, frequency);

            float4x2 minima = 2f;
            for (int u = -1; u <= 1; u++)
            {
                SmallXxHash4 hx      = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
                float4       xOffset = u - x.g0;
                for (int v = -1; v <= 1; v++)
                {
                    SmallXxHash4 hy      = hx.Eat(l.ValidateSingleStep(y.p0 + v, frequency));
                    float4       yOffset = v - y.g0;
                    for (int w = -1; w <= 1; w++)
                    {
                        SmallXxHash4 h =
                            hy.Eat(l.ValidateSingleStep(z.p0 + w, frequency));
                        float4 zOffset = w - z.g0;
                        minima = NoiseUtils.UpdateVoronoiMinima(minima, d.GetDistance(h.GetBitsAsFloats01(5, 0)  + xOffset,
                                                                           h.GetBitsAsFloats01(5, 5)  + yOffset,
                                                                           h.GetBitsAsFloats01(5, 10) + zOffset));
                        minima = NoiseUtils.UpdateVoronoiMinima(minima, d.GetDistance(h.GetBitsAsFloats01(5, 15) + xOffset,
                                                                                      h.GetBitsAsFloats01(5, 20) + yOffset,
                                                                                      h.GetBitsAsFloats01(5, 25) + zOffset));
                    }
                }
            }

            return default(TVoronoiFunction).Evaluate(d.Finalize3D(minima));
        }
    }
}