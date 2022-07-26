using AbstractInterfaces;
using Hash;
using Types;
using Unity.Mathematics;

namespace Noise.Lattices
{
    public struct Lattice2D<TLattice, TGradient> : INoise
        where TLattice : struct, ILattice 
        where TGradient : struct, IGradient
    {
        public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
        {
            var l = default(TLattice);
            LatticeSpan4
                x = l.GetLatticeSpan4(positions.c0, frequency), z = l.GetLatticeSpan4(positions.c2, frequency);

            SmallXxHash4 h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1);

            var g = default(TGradient);
            return g.EvaluateCombined(math.lerp(math.lerp(g.Evaluate(h0.Eat(z.p0), x.g0, z.g0),
                                                          g.Evaluate(h0.Eat(z.p1), x.g0, z.g1),
                                                          z.t),
                                                math.lerp(g.Evaluate(h1.Eat(z.p0), x.g1, z.g0),
                                                          g.Evaluate(h1.Eat(z.p1), x.g1, z.g1),
                                                          z.t),
                                                x.t));
        }
    }
}