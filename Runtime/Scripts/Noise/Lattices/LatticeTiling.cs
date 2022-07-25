﻿using AbstractInterfaces;
using Types;
using Unity.Mathematics;

namespace Noise.Lattices
{
    public struct LatticeTiling : ILattice
    {
        public LatticeSpan4 GetLatticeSpan4(float4 coordinates, int frequency)
        {
            coordinates *= frequency;
            float4       points = math.floor(coordinates);
            LatticeSpan4 span;
            span.p0 = (int4) points;
            span.g0 = coordinates - span.p0;
            span.g1 = span.g0     - 1f;

            span.p0 -= (int4) math.ceil(points / frequency) * frequency;
            span.p0 =  math.select(span.p0, span.p0 + frequency, span.p0 < 0);
            span.p1 =  span.p0 + 1;
            span.p1 =  math.select(span.p1, 0, span.p1 == frequency);

            span.t = coordinates - points;
            span.t = span.t * span.t * span.t * (span.t * (span.t * 6f - 15f) + 10f);
            return span;
        }

        public int4 ValidateSingleStep(int4 points, int frequency) =>
            math.select(math.select(points, 0, points == frequency), frequency - 1, points == -1);
    }
}